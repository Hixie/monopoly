
#include <list>
#include <vector>
#include <arpa/inet.h>
#include <fcntl.h>
#include <iostream>
#include <string>
#include <netinet/in.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>
#include <errno.h>
#include "message.h"
#include "exceptions.h"
#include "network.h"

//#define DEBUG_NETWORK

int sendBuffer(int socket, MessageBuffer buffer, int length)
{
#ifdef DEBUG_NETWORK
  cerr << "sending to socket " << dec << socket << ":" << endl;
  DumpBuffer(buffer, length);
#endif
  int index = 0; // how many bytes we've sent
  int result;
  while (length > 0) {
    result = send(socket, buffer+index, length, 0);
    if (result == -1) 
      break;
    index += result;
    length -= result;
  }
  return result == -1 ? -1 : 0; // return -1 on failure, 0 on success
}

Connection::Connection(Network* network, struct sockaddr_in address, fd descriptor) :
  mNetwork(network),
  mAddress(address),
  mDescriptor(descriptor)
{ }

Connection::~Connection() {
}

void Connection::Disconnect() {
  cerr << dec << mDescriptor << "--| Connection closed." << endl;
  close(mDescriptor);
  delete this;
}


DataConnection::DataConnection(Network* network, struct sockaddr_in address, fd descriptor) :
  Connection(network, address, descriptor),
  mUser(NULL),
  mCandidate(NULL),
  mLength(0),
  mProtocol(0),
  mValid(true)
{ }

DataConnection::~DataConnection() {
  if (mCandidate) {
    mCandidate->Disconnected(); // this might delete the candidate
  }
  if (mUser) {
    mUser->Disconnected();
  }
}

void DataConnection::Receive() {
  if (!mValid)
    throw NetworkInvalidError();

  int result = recv(mDescriptor, &(mBuffer[mLength]), (MAX_BUFFER_LENGTH - mLength) * sizeof(uint8), MSG_NOSIGNAL);
  if (result == 0) {
    throw NetworkDisconnectedError();
  } else if (result < 0) {
    throw NetworkReceiveError();
  }
  // we have some data
  mLength += result;

  // debug output
#ifdef DEBUG_NETWORK
  cerr << "received data from socket " << dec << mDescriptor << ", current buffer is:" << endl;
  DumpBuffer(mBuffer, mLength);
#endif

  // check that we have enough data for one packet
  while ((mLength >= 2) && (mLength >= mBuffer[1] + 2)) {
    // we have a message
    int length;
    Message* message = ParseMessage(length);

    // Shift the packet out of the buffer
    memmove(&mBuffer[0], &mBuffer[length], (MAX_BUFFER_LENGTH - length) * sizeof(uint8));
    mLength -= length;

    // check to see if it was valid
    if (message) {
      // it's valid
      if (HandleNetworkMessage(message)) {
        // we dealt with it ourselves, don't return it
        delete message;
        message = NULL;
      } else {
        mNetwork->PushMessage(message);    
      }
    } // else not valid; ParseMessage() will have complained

  }
}

Message* DataConnection::ParseMessage(int& length) {
  // We have one complete packet (at least)
  Message* message = NULL;

  // If we have an associated user, use that as the player
  int player = 0;
  if (mUser)
    player = mUser->mID;

  // Work out what it is and make a new message out of it
  int type = mBuffer[0];
  length = mBuffer[1] + 2;

  // Get a message factory
  MessageFactoryPointer factory = getMessageFactory(type);
  if (!factory) {
    // we got an unexpected packet
    // send a message back to that effect
    SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(type));
    return NULL;    
  }

  try {
    cerr << dec << mDescriptor << "->S ";
    message = factory(player, mBuffer, length);
  } catch (ParseError) {
    // we got a corrupted packet
    // send a message back to that effect
    SendAndDelete(PIMP_ERROR_UNPARSEABLE_FACTORY());
    return NULL;    
  }

  return message;
}

bool DataConnection::HandleNetworkMessage(Message* message) {
  if (mUser)
    mUser->mAFK = false;
  // connection-specific messages
  // should be kept to a minimum, imho
  switch (message->GetType()) {
  // note -- blocks ({...}) are used to keep the declaration of the
  // cast messages local, otherwise they cross case label boundaries,
  // which is a no-no.
  case PIMP_ERROR_UNEXPECTED_MESSAGE: {
    PIMP_ERROR_UNEXPECTED_MESSAGE_CAST(message, details);
    cout << "/!\\ client complained of unexpected message: " << getMessageName(details->GetField1()) << endl;
    return true;
  }
  case PIMP_ERROR_INVALID_PAYLOAD:
  case PIMP_ERROR_UNPARSEABLE: {
    // ignore it
    return true;
  }
  case PIMP_HANDSHAKE: {
    if (!mUser && !mCandidate) {
      PIMP_HANDSHAKE_CAST(message, handshake);
      if (handshake->GetField1() == 0x01) {
        SendAndDelete(PIMP_HANDSHAKE_ACKNOWLEDGE_FACTORY(mNetwork->GetGame()));
        mProtocol = 1;
      } else {
        SendAndDelete(PIMP_ERROR_UNKNOWN_PROTOCOL_FACTORY());
      }
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  case PIMP_JOIN: {
    if (mProtocol && !mUser && !mCandidate) {
      PIMP_JOIN_CAST(message, request);
      int piece = request->GetField1();
      bool playing = request->GetField2();
      char* name = request->GetField3();
      int count = mNetwork->UserCount();
      if (count >= MAX_USERS) {
        SendAndDelete(PIMP_ERROR_TOO_MANY_USERS_FACTORY());
      } else if (! mNetwork->NameAvailable(name)) {
        SendAndDelete(PIMP_ERROR_NAME_IN_USE_FACTORY());
      } else if (mNetwork->NumVoters() > 0) {
        mCandidate = mNetwork->NewCandidate(this, name, piece, playing);
      } else {
        mUser = mNetwork->NewUser(this, name, piece, playing);
      }
      free(name);
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  case PIMP_ACCEPT_JOIN: {
    if (mProtocol && mUser) {
      PIMP_ACCEPT_JOIN_CAST(message, vote);
      mNetwork->VoteInFavour(mUser, vote->GetField1());
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  case PIMP_REFUSE_JOIN: {
    if (mProtocol && mUser) {
      PIMP_REFUSE_JOIN_CAST(message, vote);
      mNetwork->VoteAgainst(mUser, vote->GetField1());
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  case PIMP_REJOIN: {
    if (mProtocol && !mUser && !mCandidate) {
      // reconnect the user
      PIMP_REJOIN_CAST(message, request);
      User* user = mNetwork->GetUser(request->GetField1());
      if (user && request->GetField2() == user->mPassword) {
        user->mAFK = false;
        user->Reconnected(this);
        SendAndDelete(PIMP_WELCOME_BACK_FACTORY(user->mID, user->mPiece,
                                                user->mName, user->mPlaying));
      } else {
        if (user)
          cout << "Player tried to reconnect but used password "
               << dec << (int)request->GetField2() << " instead of password "
               << user->mPassword << "." << endl;
        else 
          cout << "Player tried to reconnect but we don't know a user "
               << dec << (int)request->GetField1() << "." << endl;
        SendAndDelete(PIMP_ERROR_WRONG_PASSWORD_FACTORY());
      }
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  case PIMP_SWITCH_PLAY: {
    if (mProtocol && mUser && !mUser->mPlaying && !mCandidate) {
      if (mNetwork->NumVoters() > 2) {
        mCandidate = mNetwork->NewCandidate(this, mUser);
      } else {
        mUser->SwitchPlay(mNetwork);
      }
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  case PIMP_REQUEST_STATE: {
    if (mUser) {
      mNetwork->RequestState(mUser);
      return 0; // (we don't completely handle this message, we only stick our nose in)
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
      return 1;
    }
  }
  case PIMP_AFK: {
    if (mUser) {
      mUser->mAFK = true;
    } else {
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
    }
    return 1;
  }
  default:
    if (!mUser) {
      // any other message must have a user associated with it
      // otherwise, it means something went wrong with the handshake
      SendAndDelete(PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(message->GetType()));
      return 1;
    }
  };
  return 0;
}

void DataConnection::Send(MessageBuffer buffer, int length) {
  if (sendBuffer(mDescriptor, buffer, length) == -1) {
    Invalidate();
    // throw NetworkSendError(); // callers don't expect this
  }
}

void DataConnection::Send(Message* message) {
  MessageBuffer buffer;
  cerr << dec << mDescriptor << "<-S ";
  int length = message->Serialize(buffer);
  Send(buffer, length);
}

void DataConnection::SendAndDelete(Message* message) {
  Send(message);
  delete message;
}


void Listener::Receive() {
  struct sockaddr_in address;
  socklen_t sizeOfAddress = sizeof(address);
  // accept the incoming connection
  fd descriptor = accept(mDescriptor, (struct sockaddr *)&address, &sizeOfAddress);
  if (descriptor == -1) {
    throw NetworkAcceptError();
  }
  cerr << dec << descriptor << "--> Connection opened." << endl;
  fcntl(descriptor, F_SETFL, O_NONBLOCK); // make connection non-blocking
  mNetwork->AddConnection(new DataConnection(mNetwork, address, descriptor));
  return;
}


AbstractUser::AbstractUser(DataConnection* connection, char* name, int piece, bool playing) :
  mConnection(connection),
  mName(strdup(name)),
  mPiece(piece),
  mPlaying(playing),
  mID(0)
{ }

AbstractUser::~AbstractUser() {
  free(mName);
}

void AbstractUser::Disconnected() {
  mConnection = NULL;
}

void AbstractUser::Reconnected(DataConnection* connection) {
  if (mConnection) {
    mConnection->Invalidate();
    mConnection->mUser = NULL;
    mConnection->mCandidate = NULL;
    mConnection->mNetwork->RemoveConnection(mConnection);
  }
  mConnection = connection;
  Attach();
}

void AbstractUser::Send(Message* message) {
  if (mConnection) {
    mConnection->Send(message);
  }
}

void AbstractUser::SendAndDelete(Message* message) {
  Send(message);
  delete message;
}


User::User(DataConnection* connection, char* name, int piece, bool playing) :
  AbstractUser(connection, name, piece, playing),
  mCandidate(NULL),
  mLastCandidateVoted(0),
  mAFK(false)
{
  mPassword = rand();
}

void User::Reconnected(DataConnection* connection) {
  AbstractUser::Reconnected(connection);
  if (mCandidate) {
    mCandidate->Reconnected(connection);
  }
  RequestState();
}

void User::Attach() {
  mConnection->AttachUser(this);
}

void User::Send(Message* message) {
  switch (message->GetType()) {
  case PIMP_PLAYER_TAKEOVER: {
    PIMP_PLAYER_TAKEOVER_CAST(message, details);
    if (details->GetField1() == mID) {
      mPlaying = true;
    }
    break;
  }
  case PIMP_OBSERVER_BECAME_PLAYER: {
    PIMP_OBSERVER_BECAME_PLAYER_CAST(message, details);
    if (details->GetField1() == mID) {
      mPlaying = true;
    }
    break;
  }
  case PIMP_PLAYER_BECAME_OBSERVER_KICKED:
  case PIMP_PLAYER_BECAME_OBSERVER_TRANSFER:
  case PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT: {
    PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT_CAST(message, details);
    if (details->GetField1() == mID) {
      mPlaying = false;
    }
    break;
  }
  }
  AbstractUser::Send(message);
}

void User::RequestState() {
  // mConnection must be non-NULL for this to be called
  Message* message = PIMP_REQUEST_STATE_FACTORY();
  message->SetPlayer(mID);
  mConnection->mNetwork->PushMessage(message);
  mConnection->mNetwork->RequestState(this);
  if (mCandidate)
    mConnection->SendAndDelete(PIMP_JOIN_PENDING_FACTORY());
}

void User::SwitchPlay(Network* network) {
  mPlaying = true;
  Message* message = PIMP_OBSERVER_BECAME_PLAYER_FACTORY(mID);
  network->Broadcast(message);
  message->SetPlayer(mID);
  network->PushMessage(message);
}

Candidate::Candidate(DataConnection* connection, User* user, char* name, int piece, bool playing) :
  AbstractUser(connection, name, piece, playing),
  mUser(user),
  VotesInFavour(0),
  VotesAgainst(0)
{ }

Candidate::~Candidate() {
  if (mConnection) {
    mConnection->mCandidate = NULL;
  }
  if (mUser) {
    mUser->mCandidate = NULL;
  }
}

void Candidate::Disconnected() {
  if (!mUser) {
    // no point hanging on here
    Network* network = mConnection->mNetwork;
    AbstractUser::Disconnected(); // sets mConnection to NULL
    network->RemoveCandidate(this); // this will call Refused()
    delete this;
  } else {
    AbstractUser::Disconnected(); // sets mConnection to NULL
  }
}

void Candidate::Attach() {
  mConnection->AttachCandidate(this);
}

void Candidate::Accepted(Network* network) {
  if (mUser) {
    // must have been a PIMP_SWITCH_PLAY
    mUser->SwitchPlay(network);
  } else {
    // new user (mConnection cannot be NULL)
    mUser = network->NewUser(this); // this will take care of sending messages, etc
    mConnection->mUser = mUser;
  }
}

void Candidate::Refused(Network* network) {
  network->BroadcastAndDelete(PIMP_JOIN_REFUSED_FACTORY(mID, mName));
  if (mConnection) {
    mConnection->SendAndDelete(PIMP_ERROR_NOT_WELCOME_FACTORY());
  } // (else it must have been a PIMP_SWITCH_PLAY, they'll presumably
    // realise they aren't being considered any more when we don't send
    // them PIMP_JOIN_PENDING)
}



Network::Network(int port, unsigned int game) :
  mGame(game),
  mLastCandidateID(0),
  mMaxDescriptor(0)
{
  fd listener;
  int optval = 1;
  FD_ZERO(&mDescriptors); // reset the descriptor list

  // create a listener socket
  if ((listener = socket(AF_INET, SOCK_STREAM, 0)) == -1) {
    cerr << "error creating socket." << endl;
    throw NetworkSocketError();
  }

  // avoid "address already in use" error message
  if (setsockopt(listener, SOL_SOCKET, SO_REUSEADDR, &optval, sizeof(optval)) == -1) {
    cerr << "error setting socket options." << endl;
    throw NetworkSetSockOptError();
  }

  // bind listener to a port
  struct sockaddr_in address;
  address.sin_family = AF_INET;
  address.sin_addr.s_addr = INADDR_ANY;
  address.sin_port = htons(port);
  memset(&(address.sin_zero), '\0', 8);
  if (bind(listener, (struct sockaddr *)&address, sizeof(address)) == -1) {
    cerr << "error binding to socket, maybe the port is already in use" << endl;
    throw NetworkBindError();
  }

  // tell listener to start listening for connections
  if (listen(listener, SOMAXCONN) == -1) {
    cerr << "error listening on socket." << endl;
    throw NetworkListenError();
  }

  // add the listener to the list of connections
  AddConnection(new Listener(this, address, listener));
}

Network::~Network() {

  { // Remove the pending messages
  std::list<Message*>::iterator i;
  for (i = mPendingMessageQueue.begin(); i != mPendingMessageQueue.end(); ++i)
    if (*i) delete *i; }

  { // Remove the candidates
  std::list<Candidate*>::iterator i;
  for (i = mCandidates.begin(); i != mCandidates.end(); ++i)
    delete *i; }

  { // Remove the connections
  std::list<Connection*>::iterator i;
  for (i = mConnections.begin(); i != mConnections.end(); ++i)
    (*i)->Disconnect(); } // this will delete the connection too

  { // Remove the users
  std::vector<User*>::iterator i;
  for (i = mUsers.begin(); i != mUsers.end(); ++i)
    delete *i; }

}

void Network::AddConnection(Connection* connection) {
  // add the connection to the descriptor list
  FD_SET(connection->Descriptor(), &mDescriptors);
  if (mMaxDescriptor < connection->Descriptor())
    mMaxDescriptor = connection->Descriptor();
  mConnections.push_back(connection);
}

void Network::RemoveConnection(Connection* connection) {
  // remove the connection from the descriptor list
  FD_CLR(connection->Descriptor(), &mDescriptors);
  std::list<Connection*>::iterator i;
  for (i = mConnections.begin(); i != mConnections.end(); ++i) {
    if (*i == connection) {
      mConnections.erase(i);
      break;
    }
  }
  connection->Disconnect(); // this will delete it, too
}

Message* Network::GetMessage() {
  Message* message;
  while (mPendingMessageQueue.size() == 0) {
    fd_set descriptors = mDescriptors;
    if (select(mMaxDescriptor+1, &descriptors, NULL, NULL, NULL) == -1) {
      cerr << "error during select: " << dec << errno << endl;
      throw NetworkSelectError();
    }
    // deal with each pending connection
    std::list<Connection*>::iterator i;
    int count = 0;
    for (i = mConnections.begin(); i != mConnections.end(); ++i) {
      // check this connection is ready
      if (FD_ISSET((*i)->Descriptor(), &descriptors)) {
        try {
          (*i)->Receive();
        } catch (NetworkDisconnectedError) {
          std::list<Connection*>::iterator next = std::next(i);
          RemoveConnection(*i);
          i = std::prev(next);
          mPendingMessageQueue.push_back(NULL);
        }
      }
    }
  }
  message = mPendingMessageQueue.front();
  mPendingMessageQueue.pop_front();
  return message;
}

void Network::Send(Message* message) {
  Send(message, message->GetPlayer());
}

void Network::Send(Message* message, int userID) {
  if (userID) {
    User* user = GetUser(userID);
    if (user) {
      user->Send(message);
    }
  } else {
    Broadcast(message);
  }
}

void Network::SendAndDelete(Message* message, int userID) {
  Send(message, userID);
  delete message;
}

void Network::BroadcastExcept(Message* message, User* user) {
  for (int i = 0; i < mUsers.size(); ++i) {
    if (mUsers[i] != user)
      mUsers[i]->Send(message);
  }
}

void Network::BroadcastAndDeleteExcept(Message* message, User* user) {
  BroadcastExcept(message, user);
  delete message;
}

void Network::Broadcast(Message* message) {
  BroadcastExcept(message, NULL);
}

void Network::BroadcastAndDelete(Message* message) {
  BroadcastAndDeleteExcept(message, NULL);
}

void Network::PushMessage(Message* message) {
  mPendingMessageQueue.push_back(message);
}

User* Network::NewUser(Candidate* candidate) {
  return NewUser(candidate->mConnection, candidate->mName,
                 candidate->mPiece, candidate->mPlaying);
}

User* Network::NewUser(DataConnection* connection, char* name, int piece, bool playing) {
  // create user
  User* user = new User(connection, name, piece, playing);
  int id = mUsers.size() + 1;
  user->mID = id;

  // inform public
  Message* message;
  if (playing) {
    message = PIMP_WELCOME_PLAYER_FACTORY(id, piece, name);
  } else {
    message = PIMP_WELCOME_OBSERVER_FACTORY(id, piece, name);
  }
  Broadcast(message);

  // inform user
  user->SendAndDelete(PIMP_WELCOME_DETAILS_FACTORY(id, user->mPassword));

  // add user to list
  mUsers.push_back(user);

  // tell the game engine
  message->SetPlayer(id);
  PushMessage(message);

  // No need to call RequestState() because:
  //  If it's the first user there's no state to report
  //  Otherwise, it's just been accepted:
  //   If there are other pending requests, it'll be told about them whene everyone else is
  //   If there aren't any pending requests there's nothing to say.

  return user;
}

User* Network::GetUser(int id) {
  if (id <= 0 || id > mUsers.size())
    return NULL;
  return mUsers[id-1];
}

Candidate* Network::NewCandidate(DataConnection* connection, User* user, char* name, int piece, bool playing) {
  Candidate* candidate = new Candidate(connection, user, name, piece, playing);
  candidate->mID = ++mLastCandidateID;
  mCandidates.push_back(candidate);
  if (user) // if PIMP_SWITCH_PLAY, then imply vote in favour for own user
    ++candidate->VotesInFavour;
  if (connection)
    connection->SendAndDelete(PIMP_JOIN_PENDING_FACTORY());
  if (mCandidates.size() == 1) {
    NextCandidate();
  }
  return candidate;
}

void Network::RemoveCandidate(Candidate* candidate) {
  if (!mCandidates.size()) {
    return;
  }
  if (*(mCandidates.begin()) == candidate) {
    mCandidates.pop_front();
    candidate->Refused(this);
    NextCandidate();
    return;
  }
  std::list<Candidate*>::iterator i;
  for (i = mCandidates.begin(); i != mCandidates.end(); ++i) {
    if (*i == candidate) {
      mCandidates.erase(i);
      break;
    }
  }
}

void Network::NextCandidate() {
  if (!mCandidates.size())
    return;
  Candidate* candidate = mCandidates.front();
  if (candidate->mPlaying) {
    BroadcastAndDeleteExcept(PIMP_QUERY_JOIN_PLAY_FACTORY(candidate->mID, candidate->mName), candidate->mUser);
  } else {
    BroadcastAndDeleteExcept(PIMP_QUERY_JOIN_OBSERVE_FACTORY(candidate->mID, candidate->mName), candidate->mUser);
  }
}

void Network::RequestState(User* user) {
  if (mCandidates.size()) {
    Candidate* candidate = mCandidates.front();
    if (candidate->mUser != user) {
      if (candidate->mPlaying) {
        user->SendAndDelete(PIMP_QUERY_JOIN_PLAY_FACTORY(candidate->mID, candidate->mName));
      } else {
        user->SendAndDelete(PIMP_QUERY_JOIN_OBSERVE_FACTORY(candidate->mID, candidate->mName));
      }
    }
  }
}

void Network::VoteInFavour(User* user, int id) {
  if (!mCandidates.size())
    return;
  Candidate* candidate = mCandidates.front();
  if (candidate->mID != id)
    return;
  if (user->mLastCandidateVoted == id)
    return;
  if (user == candidate->mUser)
    return;
  ++candidate->VotesInFavour;
  user->mLastCandidateVoted = id;
  if (candidate->VotesInFavour >= (NumVoters() + 1) / 2) {
    mCandidates.pop_front();
    candidate->Accepted(this);
    delete candidate;
    NextCandidate();
  }
}

void Network::VoteAgainst(User* user, int id) {
  if (!mCandidates.size())
    return;
  Candidate* candidate = mCandidates.front();
  if (candidate->mID != id)
    return;
  if (user->mLastCandidateVoted == id)
    return;
  if (user == candidate->mUser)
    return;
  ++candidate->VotesAgainst;
  user->mLastCandidateVoted = id;
  if (candidate->VotesAgainst > (NumVoters()) / 2) {
    mCandidates.pop_front();
    candidate->Refused(this);
    delete candidate;
    NextCandidate();
  }
}

int Network::NumVoters() {
  int count = 0;
  std::vector<User*>::iterator i;
  for (i = mUsers.begin(); i != mUsers.end(); ++i) {
    if (!(*i)->mAFK) {
      ++count;
    }
  }
  return count;
}

int Network::NumConnections() {
  int count = 0;
  std::list<Connection*>::iterator i;
  for (i = mConnections.begin(); i != mConnections.end(); ++i)
    ++count;
  return count;
}

bool Network::NameAvailable(char* name) {
  if (strlen(name) < 1)
    return 0;
  std::vector<User*>::iterator i;
  for (i = mUsers.begin(); i != mUsers.end(); ++i) {
    if ((*i)->NameIs(name)) {
      return 0;
    }
  }
  std::list<Candidate*>::iterator j;
  for (j = mCandidates.begin(); j != mCandidates.end(); ++j) {
    if ((*j)->NameIs(name)) {
      return 0;
    }
  }
  return 1;
}

