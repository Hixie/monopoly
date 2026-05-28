#ifndef INCLUDED_EXCEPTIONS_H
#define INCLUDED_EXCEPTIONS_H

#include <stdexcept>
#include <string>

using namespace std;

#define Exception(className, parentName) \
  class className : public parentName { \
   public: \
    className(): parentName(#className) { } \
    className(const string& what_arg): parentName(what_arg) { } \
  }
   /* semicolon required at end of Exception() statement */

Exception(MessageError, std::runtime_error);
  Exception(ParseError, MessageError);
  Exception(UnknownTypeError, MessageError);
  Exception(UnexpectedMessageError, MessageError);

Exception(DataFileParseError, std::runtime_error);

Exception(NetworkError, std::runtime_error);
  Exception(NetworkSocketError, NetworkError);
  Exception(NetworkBindError, NetworkError);
  Exception(NetworkSetSockOptError, NetworkError);
  Exception(NetworkListenError, NetworkError);
  Exception(NetworkSelectError, NetworkError);
  Exception(NetworkAcceptError, NetworkError);
  Exception(NetworkConnectError, NetworkError);
  Exception(NetworkGetHostByNameError, NetworkError);
  Exception(NetworkDisconnectedError, NetworkError);
    Exception(NetworkReceiveError, NetworkDisconnectedError);
    Exception(NetworkInvalidError, NetworkDisconnectedError);
  Exception(NetworkSendError, NetworkError);

Exception(IOError, std::runtime_error);
  Exception(IOConfigurationError, IOError);

#endif
