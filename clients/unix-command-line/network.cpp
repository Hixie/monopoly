
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <unistd.h>
#include <netdb.h>
#include "network.h"
#include "../../server/message.h"
#include "../../server/exceptions.h"

Network::Network(char* server, int port) : mLength(0)
{
  // look up the server
  struct hostent* host = gethostbyname(server);
  if (!host)
    throw NetworkGetHostByNameError();

  // open a socket
  mSocket = socket(AF_INET, SOCK_STREAM, 0);
  if (mSocket == -1)
    throw NetworkSocketError();

  // connect to server
  struct sockaddr_in address;
  address.sin_family = AF_INET;
  address.sin_addr = *((struct in_addr *)host->h_addr);
  address.sin_port = htons(port);
  memset(&(address.sin_zero), '\0', 8);
  if (connect(mSocket, (struct sockaddr *)&address, sizeof(address)) == -1)
    throw NetworkConnectError();

  // cache the descriptor for select()
  FD_ZERO(&mDescriptors);
  FD_SET(mSocket, &mDescriptors);
  // also add the stdin descriptor (0) so that we return if there is input waiting
  FD_SET(0, &mDescriptors);
}

Network::~Network() { }

Message* Network::GetMessage() {
  do {

    // see if we have a message available, and if we do, return it
    if ((mLength >= 2) && (mLength >= mBuffer[1] + 2)) {
      Message* message = NULL;

      // we have at least one complete packet -- parse the first one
      int type = mBuffer[0];
      int length = mBuffer[1] + 2;

      try {

        // Get a message factory
        MessageFactoryPointer factory = getMessageFactory(type);
        if (!factory) {
          // unknown packet
          throw UnknownTypeError();
        }

        // Return the constructed message
        message = factory(0, mBuffer, length);

      } catch (...) {
        // shift the packet out of the buffer if it fails...
        memmove(&mBuffer[0], &mBuffer[length], (MAX_BUFFER_LENGTH - length) * sizeof(uint8));
        mLength -= length;
        throw;
      }

      // ...and also shift the packet out of the buffer if it succeeds.
      memmove(&mBuffer[0], &mBuffer[length], (MAX_BUFFER_LENGTH - length) * sizeof(uint8));
      mLength -= length;

      // return the message
      return message;
    }

    // no message, so let's wait for some data
    fd_set descriptors = mDescriptors;
    if (select(mSocket+1, &descriptors, NULL, NULL, NULL) == -1) {
      // it didna go too well
      throw NetworkSelectError();
    }

    // was it stdin that had pending data?
    if (FD_ISSET(0, &descriptors)) {
      // apparently so
      // note: always give local user priority over the network
      return NULL;
    }

    if (FD_ISSET(mSocket, &descriptors)) {
      // otherwise, it stands to reason that we have data. 
      // fill out buffer with said data
      int result = recv(mSocket, mBuffer, (MAX_BUFFER_LENGTH - mLength) * sizeof(uint8), MSG_NOSIGNAL);
      if (result < 0) {
        // it went bad
        throw NetworkReceiveError();
      } else if (result == 0) {
        // they hung up!
        throw NetworkDisconnectedError();
      } else {
        // got it, update the buffer
        mLength += result;
#ifdef DEBUG_NETWORK
        cerr << "received data, current buffer is:" << endl;
        DumpBuffer(mBuffer, mLength);
#endif
      }
    } else {
      throw NetworkSelectError();
    }

  } while (1); // this loop is exit either by returning from inside it, or throwing an exception
}

void Network::SendMessage(Message* message) {
  MessageBuffer buffer;
  int length = message->Serialize(buffer);
#ifdef DEBUG_NETWORK
  cerr << "sending:" << endl;
  DumpBuffer(buffer, length);
#endif
  int index = 0; // how many bytes we've sent
  int result;
  while (length > 0) {
    result = send(mSocket, buffer+index, length, 0);
    if (result == -1) 
      throw NetworkSendError();
    index += result;
    length -= result;
  }
}

void Network::SendAndDeleteMessage(Message* message) {
  SendMessage(message);
  delete message;
}
