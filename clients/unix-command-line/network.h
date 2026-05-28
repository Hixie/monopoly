#ifndef INCLUDED_NETWORK_H
#define INCLUDED_NETWORK_H

#include <netinet/in.h>
#include <sys/types.h>
#include "../../server/message.h"

#define MAX_BUFFER_LENGTH (MAX_PACKET_LENGTH*2)

typedef uint8 DoubleMessageBuffer[MAX_BUFFER_LENGTH];
typedef int fd;

class Network {
 public:
  Network(char* server, int port);
  ~Network();
  Message* GetMessage();
  void SendMessage(Message* message);
  void SendAndDeleteMessage(Message* message);
 private:
  fd mSocket;
  fd_set mDescriptors;
  DoubleMessageBuffer mBuffer;
  int mLength;
};

#endif
