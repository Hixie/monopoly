#ifndef INCLUDED_NETWORK_H
#define INCLUDED_NETWORK_H

/* PIMP: the Portable Internet Monopoly Protocol */

// http://www.ecst.csuchico.edu/~beej/guide/net/html/advanced.html
// http://www.intap.net/~drw/cpp/cpp07_01.htm

#include <list>
#include <vector>
#include <netinet/in.h>
#include <sys/types.h>
#include "message.h"

#define MAX_BUFFER_LENGTH (MAX_PACKET_LENGTH*2)

typedef uint8 DoubleMessageBuffer[MAX_BUFFER_LENGTH];
typedef int fd;

class Network;
class User;
class Candidate;

class Connection {
 public:
  Connection(Network* network, struct sockaddr_in address, fd descriptor);
  virtual ~Connection();
  virtual void Receive() = 0;
  void Disconnect();
  fd Descriptor() { return mDescriptor; }

  Network* mNetwork;

 protected:
  fd mDescriptor;
  struct sockaddr_in mAddress;
};

// DataConnections come and go
class DataConnection : public Connection {
 public:
  DataConnection(Network* network, struct sockaddr_in address, fd descriptor);
  ~DataConnection();
  void Receive();
  bool HandleNetworkMessage(Message* message);
  void Send(MessageBuffer buffer, int length);
  void Send(Message* message);
  void SendAndDelete(Message* message);
  void AttachUser(User* user) { mUser = user; }
  void AttachCandidate(Candidate* candidate) { mCandidate = candidate; }
  void Invalidate() { mValid = false; }

  // once associated with a User, the association can only be changed
  // by the death of the connection
  User* mUser;

  // candidates can go as they please
  Candidate* mCandidate;

 protected:
  Message* ParseMessage(int& length);

 private:
  DoubleMessageBuffer mBuffer;
  int mLength;
  int mProtocol;
  bool mValid;
};

// there is one Listener, and it has the same lifetime as the Network
class Listener : public Connection {
 public:
  Listener(Network* network, struct sockaddr_in address, fd descriptor): Connection(network, address, descriptor) {};
  void Receive();
};

class AbstractUser {
 public:
  AbstractUser(DataConnection* connection, char* name, int piece, bool playing);
  ~AbstractUser();
  void Disconnected();
  virtual void Reconnected(DataConnection* connection);
  virtual void Send(Message* message);
  void SendAndDelete(Message* message);
  virtual void Attach() = 0;
  bool NameIs(char* name) { return strcmp(name, mName) == 0; };

  // Changing the connection kills the previous one
  DataConnection* mConnection;

  int mID;
  char* mName; // if you use this, make sure to strdup() it
  int mPiece;
  bool mPlaying;
};

// a User has the same lifetime as a Network
class User : public AbstractUser {
 public:
  User(DataConnection* connection, char* name, int piece, bool playing);
  void Reconnected(DataConnection* connection);
  void Attach();
  void Send(Message* message);
  void RequestState();
  void SwitchPlay(Network* network);

  Candidate* mCandidate;
  unsigned int mPassword;
  unsigned int mLastCandidateVoted;
  bool mAFK;
};

// Candidates have short lifetimes
class Candidate : public AbstractUser {
 public:
  Candidate(DataConnection* connection, User* user, char* name, int piece, bool playing);
  ~Candidate();
  void Disconnected(); // also removes this if there is no user
  void Attach();
  void Accepted(Network* network); // expected to be called jsut prior to deletion
  void Refused(Network* network); // expected to be called just prior to deletion
  int VotesInFavour; // these should be manipulated directly
  int VotesAgainst;

  // Candidates can only be associated with a user at creation time
  User* mUser;
};

class Network {
 public:
  Network(int port, unsigned int game);
  ~Network();

  unsigned int GetGame() { return mGame; }

  Message* GetMessage();
  void PushMessage(Message* message); // message MUST have a non-zero Player ID
  void Send(Message* message); // gets the player id from the message
  void Send(Message* message, int userID);
  void SendAndDelete(Message* message, int userID);
  void Broadcast(Message* message); // sends the messsage to all users
  void BroadcastAndDelete(Message* message); // sends the messsage to all users
  void BroadcastExcept(Message* message, User* user); // sends the messsage to all users
  void BroadcastAndDeleteExcept(Message* message, User* user); // sends the messsage to all users

  bool UserCount() { return mUsers.size(); }
  bool NameAvailable(char* name);

  int NumConnections();

  // internal (to these classes) routines:
  void AddConnection(Connection* connection);
  void RemoveConnection(Connection* connection);

  // the next two should only be called from someone who is going to
  // set up the connection to point to the user
  User* NewUser(Candidate* candidate);
  User* NewUser(DataConnection* connection, char* name, int piece, bool playing);
  User* GetUser(int id);

  Candidate* NewCandidate(DataConnection* connection, User* user, char* name, int piece, bool playing);
  Candidate* NewCandidate(DataConnection* connection, char* name, int piece, bool playing) { return NewCandidate(connection, NULL, name, piece, playing); }
  Candidate* NewCandidate(DataConnection* connection, User* user) { return NewCandidate(connection, user, user->mName, user->mPiece, true); }
  void NextCandidate();
  void RequestState(User* user);
  void RemoveCandidate(Candidate* candidate);
  void VoteInFavour(User* user, int id);
  void VoteAgainst(User* user, int id);
  int NumVoters();

 private:
  unsigned int mGame;

  fd_set mDescriptors;
  std::list<Connection*> mConnections; // the active connections
  int mMaxDescriptor; // keep track of the highest fd as an "optimisation"

  void Select();
  std::list<Message*> mPendingMessageQueue;

  std::vector<User*> mUsers;
  std::list<Candidate*> mCandidates;
  int mLastCandidateID;
};

#endif
