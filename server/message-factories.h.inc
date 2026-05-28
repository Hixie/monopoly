 // THIS FILE IS AUTO-GENERATED: DO NOT EDIT

#include <stdlib.h>
#include <string.h>

class MessageByte : public Message {
 public:
  MessageByte(int player, uint8 type, uint8 field1);
  MessageByte(int player);
  MessageByte(MessageByte* message);
  Message* Copy() { return new MessageByte(this); }
  uint8 GetField1() { return mField1; }
  void SetField1(uint8 field1) { mField1 = field1; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField1;
};
Message* MessageByteFactory(int player, uint8 type, uint8 field1);
Message* MessageByteDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedInt : public Message {
 public:
  MessageUnsignedInt(int player, uint8 type, unsigned int field1);
  MessageUnsignedInt(int player);
  MessageUnsignedInt(MessageUnsignedInt* message);
  Message* Copy() { return new MessageUnsignedInt(this); }
  unsigned int GetField1() { return mField1; }
  void SetField1(unsigned int field1) { mField1 = field1; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField1;
};
Message* MessageUnsignedIntFactory(int player, uint8 type, unsigned int field1);
Message* MessageUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageByteBool : public MessageByte {
 public:
  MessageByteBool(int player, uint8 type, uint8 field1, bool field2);
  MessageByteBool(int player);
  MessageByteBool(MessageByteBool* message);
  Message* Copy() { return new MessageByteBool(this); }
  bool GetField2() { return mField2; }
  void SetField2(bool field2) { mField2 = field2; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  bool mField2;
};
Message* MessageByteBoolFactory(int player, uint8 type, uint8 field1, bool field2);
Message* MessageByteBoolDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByte : public MessageByte {
 public:
  MessageByteByte(int player, uint8 type, uint8 field1, uint8 field2);
  MessageByteByte(int player);
  MessageByteByte(MessageByteByte* message);
  Message* Copy() { return new MessageByteByte(this); }
  uint8 GetField2() { return mField2; }
  void SetField2(uint8 field2) { mField2 = field2; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField2;
};
Message* MessageByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2);
Message* MessageByteByteDecoder(int player, MessageBuffer buffer, int length);

class MessageByteUnsignedInt : public MessageByte {
 public:
  MessageByteUnsignedInt(int player, uint8 type, uint8 field1, unsigned int field2);
  MessageByteUnsignedInt(int player);
  MessageByteUnsignedInt(MessageByteUnsignedInt* message);
  Message* Copy() { return new MessageByteUnsignedInt(this); }
  unsigned int GetField2() { return mField2; }
  void SetField2(unsigned int field2) { mField2 = field2; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField2;
};
Message* MessageByteUnsignedIntFactory(int player, uint8 type, uint8 field1, unsigned int field2);
Message* MessageByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntByte : public MessageUnsignedInt {
 public:
  MessageUnsignedIntByte(int player, uint8 type, unsigned int field1, uint8 field2);
  MessageUnsignedIntByte(int player);
  MessageUnsignedIntByte(MessageUnsignedIntByte* message);
  Message* Copy() { return new MessageUnsignedIntByte(this); }
  uint8 GetField2() { return mField2; }
  void SetField2(uint8 field2) { mField2 = field2; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField2;
};
Message* MessageUnsignedIntByteFactory(int player, uint8 type, unsigned int field1, uint8 field2);
Message* MessageUnsignedIntByteDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntString : public MessageUnsignedInt {
 public:
  MessageUnsignedIntString(int player, uint8 type, unsigned int field1, char* field2);
  MessageUnsignedIntString(int player);
  MessageUnsignedIntString(MessageUnsignedIntString* message);
  MessageUnsignedIntString::~MessageUnsignedIntString();
  Message* Copy() { return new MessageUnsignedIntString(this); }
  char* GetField2() { return strdup(mField2); }
  void SetField2(char* field2) { free(mField2); mField2 = strdup(field2); }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  char* mField2;
};
Message* MessageUnsignedIntStringFactory(int player, uint8 type, unsigned int field1, char* field2);
Message* MessageUnsignedIntStringDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntUnsignedInt : public MessageUnsignedInt {
 public:
  MessageUnsignedIntUnsignedInt(int player, uint8 type, unsigned int field1, unsigned int field2);
  MessageUnsignedIntUnsignedInt(int player);
  MessageUnsignedIntUnsignedInt(MessageUnsignedIntUnsignedInt* message);
  Message* Copy() { return new MessageUnsignedIntUnsignedInt(this); }
  unsigned int GetField2() { return mField2; }
  void SetField2(unsigned int field2) { mField2 = field2; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField2;
};
Message* MessageUnsignedIntUnsignedIntFactory(int player, uint8 type, unsigned int field1, unsigned int field2);
Message* MessageUnsignedIntUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageByteBoolString : public MessageByteBool {
 public:
  MessageByteBoolString(int player, uint8 type, uint8 field1, bool field2, char* field3);
  MessageByteBoolString(int player);
  MessageByteBoolString(MessageByteBoolString* message);
  MessageByteBoolString::~MessageByteBoolString();
  Message* Copy() { return new MessageByteBoolString(this); }
  char* GetField3() { return strdup(mField3); }
  void SetField3(char* field3) { free(mField3); mField3 = strdup(field3); }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  char* mField3;
};
Message* MessageByteBoolStringFactory(int player, uint8 type, uint8 field1, bool field2, char* field3);
Message* MessageByteBoolStringDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteBool : public MessageByteByte {
 public:
  MessageByteByteBool(int player, uint8 type, uint8 field1, uint8 field2, bool field3);
  MessageByteByteBool(int player);
  MessageByteByteBool(MessageByteByteBool* message);
  Message* Copy() { return new MessageByteByteBool(this); }
  bool GetField3() { return mField3; }
  void SetField3(bool field3) { mField3 = field3; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  bool mField3;
};
Message* MessageByteByteBoolFactory(int player, uint8 type, uint8 field1, uint8 field2, bool field3);
Message* MessageByteByteBoolDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteByte : public MessageByteByte {
 public:
  MessageByteByteByte(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3);
  MessageByteByteByte(int player);
  MessageByteByteByte(MessageByteByteByte* message);
  Message* Copy() { return new MessageByteByteByte(this); }
  uint8 GetField3() { return mField3; }
  void SetField3(uint8 field3) { mField3 = field3; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField3;
};
Message* MessageByteByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3);
Message* MessageByteByteByteDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteString : public MessageByteByte {
 public:
  MessageByteByteString(int player, uint8 type, uint8 field1, uint8 field2, char* field3);
  MessageByteByteString(int player);
  MessageByteByteString(MessageByteByteString* message);
  MessageByteByteString::~MessageByteByteString();
  Message* Copy() { return new MessageByteByteString(this); }
  char* GetField3() { return strdup(mField3); }
  void SetField3(char* field3) { free(mField3); mField3 = strdup(field3); }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  char* mField3;
};
Message* MessageByteByteStringFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3);
Message* MessageByteByteStringDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteUnsignedInt : public MessageByteByte {
 public:
  MessageByteByteUnsignedInt(int player, uint8 type, uint8 field1, uint8 field2, unsigned int field3);
  MessageByteByteUnsignedInt(int player);
  MessageByteByteUnsignedInt(MessageByteByteUnsignedInt* message);
  Message* Copy() { return new MessageByteByteUnsignedInt(this); }
  unsigned int GetField3() { return mField3; }
  void SetField3(unsigned int field3) { mField3 = field3; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField3;
};
Message* MessageByteByteUnsignedIntFactory(int player, uint8 type, uint8 field1, uint8 field2, unsigned int field3);
Message* MessageByteByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageByteUnsignedIntUnsignedInt : public MessageByteUnsignedInt {
 public:
  MessageByteUnsignedIntUnsignedInt(int player, uint8 type, uint8 field1, unsigned int field2, unsigned int field3);
  MessageByteUnsignedIntUnsignedInt(int player);
  MessageByteUnsignedIntUnsignedInt(MessageByteUnsignedIntUnsignedInt* message);
  Message* Copy() { return new MessageByteUnsignedIntUnsignedInt(this); }
  unsigned int GetField3() { return mField3; }
  void SetField3(unsigned int field3) { mField3 = field3; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField3;
};
Message* MessageByteUnsignedIntUnsignedIntFactory(int player, uint8 type, uint8 field1, unsigned int field2, unsigned int field3);
Message* MessageByteUnsignedIntUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntByteByte : public MessageUnsignedIntByte {
 public:
  MessageUnsignedIntByteByte(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3);
  MessageUnsignedIntByteByte(int player);
  MessageUnsignedIntByteByte(MessageUnsignedIntByteByte* message);
  Message* Copy() { return new MessageUnsignedIntByteByte(this); }
  uint8 GetField3() { return mField3; }
  void SetField3(uint8 field3) { mField3 = field3; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField3;
};
Message* MessageUnsignedIntByteByteFactory(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3);
Message* MessageUnsignedIntByteByteDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntByteUnsignedInt : public MessageUnsignedIntByte {
 public:
  MessageUnsignedIntByteUnsignedInt(int player, uint8 type, unsigned int field1, uint8 field2, unsigned int field3);
  MessageUnsignedIntByteUnsignedInt(int player);
  MessageUnsignedIntByteUnsignedInt(MessageUnsignedIntByteUnsignedInt* message);
  Message* Copy() { return new MessageUnsignedIntByteUnsignedInt(this); }
  unsigned int GetField3() { return mField3; }
  void SetField3(unsigned int field3) { mField3 = field3; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField3;
};
Message* MessageUnsignedIntByteUnsignedIntFactory(int player, uint8 type, unsigned int field1, uint8 field2, unsigned int field3);
Message* MessageUnsignedIntByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteBoolByte : public MessageByteByteBool {
 public:
  MessageByteByteBoolByte(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4);
  MessageByteByteBoolByte(int player);
  MessageByteByteBoolByte(MessageByteByteBoolByte* message);
  Message* Copy() { return new MessageByteByteBoolByte(this); }
  uint8 GetField4() { return mField4; }
  void SetField4(uint8 field4) { mField4 = field4; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField4;
};
Message* MessageByteByteBoolByteFactory(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4);
Message* MessageByteByteBoolByteDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteByteByte : public MessageByteByteByte {
 public:
  MessageByteByteByteByte(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, uint8 field4);
  MessageByteByteByteByte(int player);
  MessageByteByteByteByte(MessageByteByteByteByte* message);
  Message* Copy() { return new MessageByteByteByteByte(this); }
  uint8 GetField4() { return mField4; }
  void SetField4(uint8 field4) { mField4 = field4; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField4;
};
Message* MessageByteByteByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, uint8 field4);
Message* MessageByteByteByteByteDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteByteUnsignedInt : public MessageByteByteByte {
 public:
  MessageByteByteByteUnsignedInt(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, unsigned int field4);
  MessageByteByteByteUnsignedInt(int player);
  MessageByteByteByteUnsignedInt(MessageByteByteByteUnsignedInt* message);
  Message* Copy() { return new MessageByteByteByteUnsignedInt(this); }
  unsigned int GetField4() { return mField4; }
  void SetField4(unsigned int field4) { mField4 = field4; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField4;
};
Message* MessageByteByteByteUnsignedIntFactory(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, unsigned int field4);
Message* MessageByteByteByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteStringBool : public MessageByteByteString {
 public:
  MessageByteByteStringBool(int player, uint8 type, uint8 field1, uint8 field2, char* field3, bool field4);
  MessageByteByteStringBool(int player);
  MessageByteByteStringBool(MessageByteByteStringBool* message);
  Message* Copy() { return new MessageByteByteStringBool(this); }
  bool GetField4() { return mField4; }
  void SetField4(bool field4) { mField4 = field4; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  bool mField4;
};
Message* MessageByteByteStringBoolFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, bool field4);
Message* MessageByteByteStringBoolDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteStringByte : public MessageByteByteString {
 public:
  MessageByteByteStringByte(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4);
  MessageByteByteStringByte(int player);
  MessageByteByteStringByte(MessageByteByteStringByte* message);
  Message* Copy() { return new MessageByteByteStringByte(this); }
  uint8 GetField4() { return mField4; }
  void SetField4(uint8 field4) { mField4 = field4; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField4;
};
Message* MessageByteByteStringByteFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4);
Message* MessageByteByteStringByteDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntByteByteByte : public MessageUnsignedIntByteByte {
 public:
  MessageUnsignedIntByteByteByte(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4);
  MessageUnsignedIntByteByteByte(int player);
  MessageUnsignedIntByteByteByte(MessageUnsignedIntByteByteByte* message);
  Message* Copy() { return new MessageUnsignedIntByteByteByte(this); }
  uint8 GetField4() { return mField4; }
  void SetField4(uint8 field4) { mField4 = field4; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField4;
};
Message* MessageUnsignedIntByteByteByteFactory(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4);
Message* MessageUnsignedIntByteByteByteDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteBoolByteByte : public MessageByteByteBoolByte {
 public:
  MessageByteByteBoolByteByte(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4, uint8 field5);
  MessageByteByteBoolByteByte(int player);
  MessageByteByteBoolByteByte(MessageByteByteBoolByteByte* message);
  Message* Copy() { return new MessageByteByteBoolByteByte(this); }
  uint8 GetField5() { return mField5; }
  void SetField5(uint8 field5) { mField5 = field5; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField5;
};
Message* MessageByteByteBoolByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4, uint8 field5);
Message* MessageByteByteBoolByteByteDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteStringByteUnsignedInt : public MessageByteByteStringByte {
 public:
  MessageByteByteStringByteUnsignedInt(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5);
  MessageByteByteStringByteUnsignedInt(int player);
  MessageByteByteStringByteUnsignedInt(MessageByteByteStringByteUnsignedInt* message);
  Message* Copy() { return new MessageByteByteStringByteUnsignedInt(this); }
  unsigned int GetField5() { return mField5; }
  void SetField5(unsigned int field5) { mField5 = field5; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField5;
};
Message* MessageByteByteStringByteUnsignedIntFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5);
Message* MessageByteByteStringByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageUnsignedIntByteByteByteUnsignedInt : public MessageUnsignedIntByteByteByte {
 public:
  MessageUnsignedIntByteByteByteUnsignedInt(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4, unsigned int field5);
  MessageUnsignedIntByteByteByteUnsignedInt(int player);
  MessageUnsignedIntByteByteByteUnsignedInt(MessageUnsignedIntByteByteByteUnsignedInt* message);
  Message* Copy() { return new MessageUnsignedIntByteByteByteUnsignedInt(this); }
  unsigned int GetField5() { return mField5; }
  void SetField5(unsigned int field5) { mField5 = field5; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  unsigned int mField5;
};
Message* MessageUnsignedIntByteByteByteUnsignedIntFactory(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4, unsigned int field5);
Message* MessageUnsignedIntByteByteByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length);

class MessageByteByteStringByteUnsignedIntByte : public MessageByteByteStringByteUnsignedInt {
 public:
  MessageByteByteStringByteUnsignedIntByte(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5, uint8 field6);
  MessageByteByteStringByteUnsignedIntByte(int player);
  MessageByteByteStringByteUnsignedIntByte(MessageByteByteStringByteUnsignedIntByte* message);
  Message* Copy() { return new MessageByteByteStringByteUnsignedIntByte(this); }
  uint8 GetField6() { return mField6; }
  void SetField6(uint8 field6) { mField6 = field6; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mField6;
};
Message* MessageByteByteStringByteUnsignedIntByteFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5, uint8 field6);
Message* MessageByteByteStringByteUnsignedIntByteDecoder(int player, MessageBuffer buffer, int length);


// C function pointer syntax is retarded
// define a typedef to make it less cumbersome
typedef Message* (*MessageFactoryPointer) (int, MessageBuffer, int);

#define PIMP_ACCEPT_JOIN 0x08
#define PIMP_ACCEPT_JOIN_FACTORY(field1) MessageUnsignedIntFactory(0, 0x08, field1)
#define PIMP_ACCEPT_JOIN_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_AFK 0xD5
#define PIMP_AFK_FACTORY() MessageFactory(0, 0xD5)
#define PIMP_AFK_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_AUCTION_PROPERTY 0x32
#define PIMP_AUCTION_PROPERTY_FACTORY() MessageFactory(0, 0x32)
#define PIMP_AUCTION_PROPERTY_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_BANKRUPT_TRANSACTION 0x7A
#define PIMP_BANKRUPT_TRANSACTION_FACTORY(field1) MessageUnsignedIntFactory(0, 0x7A, field1)
#define PIMP_BANKRUPT_TRANSACTION_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_BID 0x34
#define PIMP_BID_FACTORY(field1) MessageUnsignedIntFactory(0, 0x34, field1)
#define PIMP_BID_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_BUY_PROPERTY 0x31
#define PIMP_BUY_PROPERTY_FACTORY() MessageFactory(0, 0x31)
#define PIMP_BUY_PROPERTY_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_CARD_COLLECT_OR_RETRY 0x41
#define PIMP_CARD_COLLECT_OR_RETRY_FACTORY(field1) MessageByteFactory(0, 0x41, field1)
#define PIMP_CARD_COLLECT_OR_RETRY_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_CARD_MOVED_PLAYER 0x25
#define PIMP_CARD_MOVED_PLAYER_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x25, field1, field2, field3)
#define PIMP_CARD_MOVED_PLAYER_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_CARD_SELECT_COLLECT 0x42
#define PIMP_CARD_SELECT_COLLECT_FACTORY() MessageFactory(0, 0x42)
#define PIMP_CARD_SELECT_COLLECT_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_CARD_SELECT_RETRY 0x43
#define PIMP_CARD_SELECT_RETRY_FACTORY() MessageFactory(0, 0x43)
#define PIMP_CARD_SELECT_RETRY_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_CLAIM_GO 0x2C
#define PIMP_CLAIM_GO_FACTORY(field1) MessageByteFactory(0, 0x2C, field1)
#define PIMP_CLAIM_GO_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_CLAIM_RENT 0x2B
#define PIMP_CLAIM_RENT_FACTORY(field1, field2) MessageByteByteFactory(0, 0x2B, field1, field2)
#define PIMP_CLAIM_RENT_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_CONSTRUCTION_TAKES_CASH 0x84
#define PIMP_CONSTRUCTION_TAKES_CASH_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0x84, field1, field2)
#define PIMP_CONSTRUCTION_TAKES_CASH_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_DELTA_BANK 0xC4
#define PIMP_DELTA_BANK_FACTORY(field1, field2) MessageByteByteFactory(0, 0xC4, field1, field2)
#define PIMP_DELTA_BANK_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_DELTA_CARD 0xC2
#define PIMP_DELTA_CARD_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0xC2, field1, field2, field3)
#define PIMP_DELTA_CARD_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_DELTA_CASH 0xC0
#define PIMP_DELTA_CASH_FACTORY(field1, field2, field3) MessageByteByteUnsignedIntFactory(0, 0xC0, field1, field2, field3)
#define PIMP_DELTA_CASH_CAST(message, cast) MessageByteByteUnsignedInt* cast = static_cast<MessageByteByteUnsignedInt*> (message)
#define PIMP_DELTA_HOUSES_PURCHASED 0xC3
#define PIMP_DELTA_HOUSES_PURCHASED_FACTORY(field1, field2, field3, field4) MessageByteByteByteByteFactory(0, 0xC3, field1, field2, field3, field4)
#define PIMP_DELTA_HOUSES_PURCHASED_CAST(message, cast) MessageByteByteByteByte* cast = static_cast<MessageByteByteByteByte*> (message)
#define PIMP_DELTA_POT 0xCF
#define PIMP_DELTA_POT_FACTORY(field1) MessageUnsignedIntFactory(0, 0xCF, field1)
#define PIMP_DELTA_POT_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_DELTA_PROPERTY 0xC1
#define PIMP_DELTA_PROPERTY_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0xC1, field1, field2, field3)
#define PIMP_DELTA_PROPERTY_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_DELTA_PROPERTY_MORTGAGED 0xC5
#define PIMP_DELTA_PROPERTY_MORTGAGED_FACTORY(field1) MessageByteFactory(0, 0xC5, field1)
#define PIMP_DELTA_PROPERTY_MORTGAGED_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_DELTA_PROPERTY_UNMORTGAGED 0xC6
#define PIMP_DELTA_PROPERTY_UNMORTGAGED_FACTORY(field1) MessageByteFactory(0, 0xC6, field1)
#define PIMP_DELTA_PROPERTY_UNMORTGAGED_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_DESTRUCTION_GIVES_CASH 0x85
#define PIMP_DESTRUCTION_GIVES_CASH_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0x85, field1, field2)
#define PIMP_DESTRUCTION_GIVES_CASH_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_DICE_MOVED_PLAYER 0x24
#define PIMP_DICE_MOVED_PLAYER_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x24, field1, field2, field3)
#define PIMP_DICE_MOVED_PLAYER_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_DICE_ROLLED 0x22
#define PIMP_DICE_ROLLED_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x22, field1, field2, field3)
#define PIMP_DICE_ROLLED_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION 0xEC
#define PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION_FACTORY(field1) MessageUnsignedIntFactory(0, 0xEC, field1)
#define PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER 0x98
#define PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER_FACTORY(field1) MessageByteFactory(0, 0x98, field1)
#define PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_HOUSES_MORTGAGED 0x93
#define PIMP_ERROR_HOUSES_MORTGAGED_FACTORY(field1) MessageByteFactory(0, 0x93, field1)
#define PIMP_ERROR_HOUSES_MORTGAGED_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_HOUSES_NEED_MONOPOLY 0x92
#define PIMP_ERROR_HOUSES_NEED_MONOPOLY_FACTORY(field1) MessageByteFactory(0, 0x92, field1)
#define PIMP_ERROR_HOUSES_NEED_MONOPOLY_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP 0x94
#define PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP_FACTORY(field1) MessageByteFactory(0, 0x94, field1)
#define PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_HOUSES_NOT_OWNED 0x91
#define PIMP_ERROR_HOUSES_NOT_OWNED_FACTORY(field1) MessageByteFactory(0, 0x91, field1)
#define PIMP_ERROR_HOUSES_NOT_OWNED_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_HOUSES_NOT_YOUR_TURN 0x95
#define PIMP_ERROR_HOUSES_NOT_YOUR_TURN_FACTORY() MessageFactory(0, 0x95)
#define PIMP_ERROR_HOUSES_NOT_YOUR_TURN_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT 0x97
#define PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT_FACTORY(field1, field2) MessageByteByteFactory(0, 0x97, field1, field2)
#define PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_ERROR_HOUSES_TERMITES 0x9B
#define PIMP_ERROR_HOUSES_TERMITES_FACTORY() MessageFactory(0, 0x9B)
#define PIMP_ERROR_HOUSES_TERMITES_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_HOUSES_TOO_EXPENSIVE 0x96
#define PIMP_ERROR_HOUSES_TOO_EXPENSIVE_FACTORY(field1) MessageUnsignedIntFactory(0, 0x96, field1)
#define PIMP_ERROR_HOUSES_TOO_EXPENSIVE_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_ERROR_HOUSES_UNBALANCED 0x99
#define PIMP_ERROR_HOUSES_UNBALANCED_FACTORY(field1) MessageByteFactory(0, 0x99, field1)
#define PIMP_ERROR_HOUSES_UNBALANCED_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH 0x9A
#define PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH_FACTORY(field1) MessageUnsignedIntFactory(0, 0x9A, field1)
#define PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_ERROR_INVALID_GO_CLAIM 0xE1
#define PIMP_ERROR_INVALID_GO_CLAIM_FACTORY() MessageFactory(0, 0xE1)
#define PIMP_ERROR_INVALID_GO_CLAIM_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_INVALID_PAYLOAD 0xFC
#define PIMP_ERROR_INVALID_PAYLOAD_FACTORY(field1) MessageByteFactory(0, 0xFC, field1)
#define PIMP_ERROR_INVALID_PAYLOAD_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_INVALID_RENT_CLAIM 0xE0
#define PIMP_ERROR_INVALID_RENT_CLAIM_FACTORY() MessageFactory(0, 0xE0)
#define PIMP_ERROR_INVALID_RENT_CLAIM_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE 0xE3
#define PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0xE3, field1, field2)
#define PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_ERROR_NAME_IN_USE 0xF2
#define PIMP_ERROR_NAME_IN_USE_FACTORY() MessageFactory(0, 0xF2)
#define PIMP_ERROR_NAME_IN_USE_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_NOT_WELCOME 0xF3
#define PIMP_ERROR_NOT_WELCOME_FACTORY() MessageFactory(0, 0xF3)
#define PIMP_ERROR_NOT_WELCOME_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_PROPERTY_TOO_EXPENSIVE 0xE2
#define PIMP_ERROR_PROPERTY_TOO_EXPENSIVE_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0xE2, field1, field2)
#define PIMP_ERROR_PROPERTY_TOO_EXPENSIVE_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH 0xEE
#define PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0xEE, field1, field2)
#define PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_ERROR_TOO_MANY_USERS 0xF1
#define PIMP_ERROR_TOO_MANY_USERS_FACTORY() MessageFactory(0, 0xF1)
#define PIMP_ERROR_TOO_MANY_USERS_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED 0xEB
#define PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED_FACTORY(field1) MessageUnsignedIntFactory(0, 0xEB, field1)
#define PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED 0xE6
#define PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0xE6, field1, field2)
#define PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_ERROR_TRANSACTION_NOT_BANKRUPT 0xEA
#define PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0xEA, field1, field2)
#define PIMP_ERROR_TRANSACTION_NOT_BANKRUPT_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_ERROR_TRANSACTION_NOT_SUITABLE 0xE7
#define PIMP_ERROR_TRANSACTION_NOT_SUITABLE_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0xE7, field1, field2)
#define PIMP_ERROR_TRANSACTION_NOT_SUITABLE_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE 0xE8
#define PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0xE8, field1, field2)
#define PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED 0xE5
#define PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0xE5, field1, field2)
#define PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE 0xE4
#define PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0xE4, field1, field2)
#define PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH 0xE9
#define PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0xE9, field1, field2)
#define PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_ERROR_UNEXPECTED_MESSAGE 0xFE
#define PIMP_ERROR_UNEXPECTED_MESSAGE_FACTORY(field1) MessageByteFactory(0, 0xFE, field1)
#define PIMP_ERROR_UNEXPECTED_MESSAGE_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_ERROR_UNKNOWN_PROTOCOL 0xF0
#define PIMP_ERROR_UNKNOWN_PROTOCOL_FACTORY() MessageFactory(0, 0xF0)
#define PIMP_ERROR_UNKNOWN_PROTOCOL_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_UNPARSEABLE 0xFD
#define PIMP_ERROR_UNPARSEABLE_FACTORY() MessageFactory(0, 0xFD)
#define PIMP_ERROR_UNPARSEABLE_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ERROR_WRONG_PASSWORD 0xF4
#define PIMP_ERROR_WRONG_PASSWORD_FACTORY() MessageFactory(0, 0xF4)
#define PIMP_ERROR_WRONG_PASSWORD_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_GOING_TO_JAIL 0x29
#define PIMP_GOING_TO_JAIL_FACTORY(field1, field2) MessageByteByteFactory(0, 0x29, field1, field2)
#define PIMP_GOING_TO_JAIL_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_GOT_CARD 0x40
#define PIMP_GOT_CARD_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x40, field1, field2, field3)
#define PIMP_GOT_CARD_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_HANDSHAKE 0x00
#define PIMP_HANDSHAKE_FACTORY(field1) MessageByteFactory(0, 0x00, field1)
#define PIMP_HANDSHAKE_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_HANDSHAKE_ACKNOWLEDGE 0x01
#define PIMP_HANDSHAKE_ACKNOWLEDGE_FACTORY(field1) MessageUnsignedIntFactory(0, 0x01, field1)
#define PIMP_HANDSHAKE_ACKNOWLEDGE_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_JAIL_FREE 0x4D
#define PIMP_JAIL_FREE_FACTORY(field1) MessageByteFactory(0, 0x4D, field1)
#define PIMP_JAIL_FREE_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_JAIL_PAY_BAIL 0x4B
#define PIMP_JAIL_PAY_BAIL_FACTORY() MessageFactory(0, 0x4B)
#define PIMP_JAIL_PAY_BAIL_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_JAIL_PAY_OR_ROLL 0x4A
#define PIMP_JAIL_PAY_OR_ROLL_FACTORY(field1, field2, field3) MessageByteByteBoolFactory(0, 0x4A, field1, field2, field3)
#define PIMP_JAIL_PAY_OR_ROLL_CAST(message, cast) MessageByteByteBool* cast = static_cast<MessageByteByteBool*> (message)
#define PIMP_JAIL_ROLL_DICE 0x4C
#define PIMP_JAIL_ROLL_DICE_FACTORY() MessageFactory(0, 0x4C)
#define PIMP_JAIL_ROLL_DICE_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_JOIN 0x02
#define PIMP_JOIN_FACTORY(field1, field2, field3) MessageByteBoolStringFactory(0, 0x02, field1, field2, field3)
#define PIMP_JOIN_CAST(message, cast) MessageByteBoolString* cast = static_cast<MessageByteBoolString*> (message)
#define PIMP_JOIN_PENDING 0x10
#define PIMP_JOIN_PENDING_FACTORY() MessageFactory(0, 0x10)
#define PIMP_JOIN_PENDING_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_JOIN_REFUSED 0x0F
#define PIMP_JOIN_REFUSED_FACTORY(field1, field2) MessageUnsignedIntStringFactory(0, 0x0F, field1, field2)
#define PIMP_JOIN_REFUSED_CAST(message, cast) MessageUnsignedIntString* cast = static_cast<MessageUnsignedIntString*> (message)
#define PIMP_KICK 0xD3
#define PIMP_KICK_FACTORY(field1) MessageByteFactory(0, 0xD3, field1)
#define PIMP_KICK_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_LINK_DEAD 0xD2
#define PIMP_LINK_DEAD_FACTORY(field1) MessageByteFactory(0, 0xD2, field1)
#define PIMP_LINK_DEAD_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_MONEY_BEING_HELD_IN_ESCROW 0xED
#define PIMP_MONEY_BEING_HELD_IN_ESCROW_FACTORY(field1, field2, field3) MessageByteUnsignedIntUnsignedIntFactory(0, 0xED, field1, field2, field3)
#define PIMP_MONEY_BEING_HELD_IN_ESCROW_CAST(message, cast) MessageByteUnsignedIntUnsignedInt* cast = static_cast<MessageByteUnsignedIntUnsignedInt*> (message)
#define PIMP_MORTGAGE_PROPERTY 0x3B
#define PIMP_MORTGAGE_PROPERTY_FACTORY(field1) MessageByteFactory(0, 0x3B, field1)
#define PIMP_MORTGAGE_PROPERTY_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_NO_BID 0x36
#define PIMP_NO_BID_FACTORY() MessageFactory(0, 0x36)
#define PIMP_NO_BID_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_OBSERVER_BECAME_PLAYER 0x0E
#define PIMP_OBSERVER_BECAME_PLAYER_FACTORY(field1) MessageByteFactory(0, 0x0E, field1)
#define PIMP_OBSERVER_BECAME_PLAYER_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PING 0xD0
#define PIMP_PING_FACTORY(field1) MessageUnsignedIntFactory(0, 0xD0, field1)
#define PIMP_PING_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT 0xDB
#define PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT_FACTORY(field1) MessageByteFactory(0, 0xDB, field1)
#define PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PLAYER_BECAME_OBSERVER_KICKED 0xDC
#define PIMP_PLAYER_BECAME_OBSERVER_KICKED_FACTORY(field1) MessageByteFactory(0, 0xDC, field1)
#define PIMP_PLAYER_BECAME_OBSERVER_KICKED_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PLAYER_BECAME_OBSERVER_TRANSFER 0xDA
#define PIMP_PLAYER_BECAME_OBSERVER_TRANSFER_FACTORY(field1) MessageByteFactory(0, 0xDA, field1)
#define PIMP_PLAYER_BECAME_OBSERVER_TRANSFER_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PLAYER_CLAIMED_GO 0x87
#define PIMP_PLAYER_CLAIMED_GO_FACTORY(field1, field2, field3) MessageByteByteUnsignedIntFactory(0, 0x87, field1, field2, field3)
#define PIMP_PLAYER_CLAIMED_GO_CAST(message, cast) MessageByteByteUnsignedInt* cast = static_cast<MessageByteByteUnsignedInt*> (message)
#define PIMP_PLAYER_CLAIMED_RENT 0x86
#define PIMP_PLAYER_CLAIMED_RENT_FACTORY(field1, field2, field3, field4) MessageByteByteByteUnsignedIntFactory(0, 0x86, field1, field2, field3, field4)
#define PIMP_PLAYER_CLAIMED_RENT_CAST(message, cast) MessageByteByteByteUnsignedInt* cast = static_cast<MessageByteByteByteUnsignedInt*> (message)
#define PIMP_PLAYER_LANDING_ON_SQUARE 0x28
#define PIMP_PLAYER_LANDING_ON_SQUARE_FACTORY(field1, field2) MessageByteByteFactory(0, 0x28, field1, field2)
#define PIMP_PLAYER_LANDING_ON_SQUARE_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_PLAYER_PASSING_BY_SQUARE 0x27
#define PIMP_PLAYER_PASSING_BY_SQUARE_FACTORY(field1, field2) MessageByteByteFactory(0, 0x27, field1, field2)
#define PIMP_PLAYER_PASSING_BY_SQUARE_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_PLAYER_TAKEOVER 0x0C
#define PIMP_PLAYER_TAKEOVER_FACTORY(field1, field2) MessageByteByteFactory(0, 0x0C, field1, field2)
#define PIMP_PLAYER_TAKEOVER_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_PLAYER_WON 0xDF
#define PIMP_PLAYER_WON_FACTORY(field1) MessageByteFactory(0, 0xDF, field1)
#define PIMP_PLAYER_WON_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PONG 0xD1
#define PIMP_PONG_FACTORY(field1) MessageUnsignedIntFactory(0, 0xD1, field1)
#define PIMP_PONG_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_PROPERTY_AUCTION 0x33
#define PIMP_PROPERTY_AUCTION_FACTORY(field1) MessageByteFactory(0, 0x33, field1)
#define PIMP_PROPERTY_AUCTION_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PROPERTY_AUCTION_BID 0x35
#define PIMP_PROPERTY_AUCTION_BID_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0x35, field1, field2)
#define PIMP_PROPERTY_AUCTION_BID_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_PROPERTY_AUCTION_NO_BID 0x37
#define PIMP_PROPERTY_AUCTION_NO_BID_FACTORY(field1) MessageByteFactory(0, 0x37, field1)
#define PIMP_PROPERTY_AUCTION_NO_BID_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PROPERTY_AUCTION_VOID 0x39
#define PIMP_PROPERTY_AUCTION_VOID_FACTORY() MessageFactory(0, 0x39)
#define PIMP_PROPERTY_AUCTION_VOID_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_PROPERTY_AUCTION_WON 0x38
#define PIMP_PROPERTY_AUCTION_WON_FACTORY(field1) MessageByteFactory(0, 0x38, field1)
#define PIMP_PROPERTY_AUCTION_WON_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_PROPERTY_SALE 0x30
#define PIMP_PROPERTY_SALE_FACTORY(field1, field2, field3) MessageByteByteUnsignedIntFactory(0, 0x30, field1, field2, field3)
#define PIMP_PROPERTY_SALE_CAST(message, cast) MessageByteByteUnsignedInt* cast = static_cast<MessageByteByteUnsignedInt*> (message)
#define PIMP_PURCHASE_HOUSES 0x90
#define PIMP_PURCHASE_HOUSES_FACTORY(count, properties, amountHouses, amountHotels) MessageHouseChangeListFactory(0, 0x90, count, properties, amountHouses, amountHotels)
#define PIMP_PURCHASE_HOUSES_CAST(message, cast) MessageHouseChangeList* cast = static_cast<MessageHouseChangeList*> (message)
#define PIMP_QUERY_JOIN_OBSERVE 0x07
#define PIMP_QUERY_JOIN_OBSERVE_FACTORY(field1, field2) MessageUnsignedIntStringFactory(0, 0x07, field1, field2)
#define PIMP_QUERY_JOIN_OBSERVE_CAST(message, cast) MessageUnsignedIntString* cast = static_cast<MessageUnsignedIntString*> (message)
#define PIMP_QUERY_JOIN_PLAY 0x06
#define PIMP_QUERY_JOIN_PLAY_FACTORY(field1, field2) MessageUnsignedIntStringFactory(0, 0x06, field1, field2)
#define PIMP_QUERY_JOIN_PLAY_CAST(message, cast) MessageUnsignedIntString* cast = static_cast<MessageUnsignedIntString*> (message)
#define PIMP_REFUSE_JOIN 0x09
#define PIMP_REFUSE_JOIN_FACTORY(field1) MessageUnsignedIntFactory(0, 0x09, field1)
#define PIMP_REFUSE_JOIN_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_REJOIN 0x0A
#define PIMP_REJOIN_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0x0A, field1, field2)
#define PIMP_REJOIN_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_RENT_COLLECTION_MORATORIUM 0x2E
#define PIMP_RENT_COLLECTION_MORATORIUM_FACTORY() MessageFactory(0, 0x2E)
#define PIMP_RENT_COLLECTION_MORATORIUM_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_REQUEST_STATE 0x11
#define PIMP_REQUEST_STATE_FACTORY() MessageFactory(0, 0x11)
#define PIMP_REQUEST_STATE_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_ROLL_AGAIN 0x2F
#define PIMP_ROLL_AGAIN_FACTORY(field1) MessageByteFactory(0, 0x2F, field1)
#define PIMP_ROLL_AGAIN_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_SQUARE_GIVES_CARD 0x82
#define PIMP_SQUARE_GIVES_CARD_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x82, field1, field2, field3)
#define PIMP_SQUARE_GIVES_CARD_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_SQUARE_GIVES_CASH 0x80
#define PIMP_SQUARE_GIVES_CASH_FACTORY(field1, field2, field3) MessageByteByteUnsignedIntFactory(0, 0x80, field1, field2, field3)
#define PIMP_SQUARE_GIVES_CASH_CAST(message, cast) MessageByteByteUnsignedInt* cast = static_cast<MessageByteByteUnsignedInt*> (message)
#define PIMP_SQUARE_MOVED_PLAYER 0x26
#define PIMP_SQUARE_MOVED_PLAYER_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x26, field1, field2, field3)
#define PIMP_SQUARE_MOVED_PLAYER_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_SQUARE_TAKES_CARD 0x83
#define PIMP_SQUARE_TAKES_CARD_FACTORY(field1, field2, field3) MessageByteByteByteFactory(0, 0x83, field1, field2, field3)
#define PIMP_SQUARE_TAKES_CARD_CAST(message, cast) MessageByteByteByte* cast = static_cast<MessageByteByteByte*> (message)
#define PIMP_SQUARE_TAKES_CASH 0x81
#define PIMP_SQUARE_TAKES_CASH_FACTORY(field1, field2, field3) MessageByteByteUnsignedIntFactory(0, 0x81, field1, field2, field3)
#define PIMP_SQUARE_TAKES_CASH_CAST(message, cast) MessageByteByteUnsignedInt* cast = static_cast<MessageByteByteUnsignedInt*> (message)
#define PIMP_START_OF_TURN 0x20
#define PIMP_START_OF_TURN_FACTORY(field1, field2) MessageByteBoolFactory(0, 0x20, field1, field2)
#define PIMP_START_OF_TURN_CAST(message, cast) MessageByteBool* cast = static_cast<MessageByteBool*> (message)
#define PIMP_STATE_BOARD 0x12
#define PIMP_STATE_BOARD_FACTORY(field1) MessageByteFactory(0, 0x12, field1)
#define PIMP_STATE_BOARD_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_STATE_CARD 0x16
#define PIMP_STATE_CARD_FACTORY(field1, field2) MessageByteByteFactory(0, 0x16, field1, field2)
#define PIMP_STATE_CARD_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_STATE_OBSERVER 0x14
#define PIMP_STATE_OBSERVER_FACTORY(field1, field2, field3) MessageByteByteStringFactory(0, 0x14, field1, field2, field3)
#define PIMP_STATE_OBSERVER_CAST(message, cast) MessageByteByteString* cast = static_cast<MessageByteByteString*> (message)
#define PIMP_STATE_PLAYER 0x13
#define PIMP_STATE_PLAYER_FACTORY(field1, field2, field3, field4, field5, field6) MessageByteByteStringByteUnsignedIntByteFactory(0, 0x13, field1, field2, field3, field4, field5, field6)
#define PIMP_STATE_PLAYER_CAST(message, cast) MessageByteByteStringByteUnsignedIntByte* cast = static_cast<MessageByteByteStringByteUnsignedIntByte*> (message)
#define PIMP_STATE_POT 0x1F
#define PIMP_STATE_POT_FACTORY(field1) MessageUnsignedIntFactory(0, 0x1F, field1)
#define PIMP_STATE_POT_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_STATE_PROPERTY 0x15
#define PIMP_STATE_PROPERTY_FACTORY(field1, field2, field3, field4, field5) MessageByteByteBoolByteByteFactory(0, 0x15, field1, field2, field3, field4, field5)
#define PIMP_STATE_PROPERTY_CAST(message, cast) MessageByteByteBoolByteByte* cast = static_cast<MessageByteByteBoolByteByte*> (message)
#define PIMP_SWITCH_PLAY 0x0D
#define PIMP_SWITCH_PLAY_FACTORY() MessageFactory(0, 0x0D)
#define PIMP_SWITCH_PLAY_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_TAX_PAY_FLAT_FEE 0x47
#define PIMP_TAX_PAY_FLAT_FEE_FACTORY() MessageFactory(0, 0x47)
#define PIMP_TAX_PAY_FLAT_FEE_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_TAX_PAY_TEN_PERCENT 0x46
#define PIMP_TAX_PAY_TEN_PERCENT_FACTORY() MessageFactory(0, 0x46)
#define PIMP_TAX_PAY_TEN_PERCENT_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_TAX_SELECT_OPTION 0x45
#define PIMP_TAX_SELECT_OPTION_FACTORY(field1, field2) MessageByteByteFactory(0, 0x45, field1, field2)
#define PIMP_TAX_SELECT_OPTION_CAST(message, cast) MessageByteByte* cast = static_cast<MessageByteByte*> (message)
#define PIMP_THREE_DOUBLES 0x23
#define PIMP_THREE_DOUBLES_FACTORY(field1) MessageByteFactory(0, 0x23, field1)
#define PIMP_THREE_DOUBLES_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_THROW_DICE 0x21
#define PIMP_THROW_DICE_FACTORY() MessageFactory(0, 0x21)
#define PIMP_THROW_DICE_CAST(message, cast) Message* cast = static_cast<Message*> (message)
#define PIMP_TRANSACTION_ADD_CARD 0x69
#define PIMP_TRANSACTION_ADD_CARD_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x69, field1, field2)
#define PIMP_TRANSACTION_ADD_CARD_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_ADD_PROPERTY 0x63
#define PIMP_TRANSACTION_ADD_PROPERTY_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x63, field1, field2)
#define PIMP_TRANSACTION_ADD_PROPERTY_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_AGREE 0x76
#define PIMP_TRANSACTION_AGREE_FACTORY(field1) MessageUnsignedIntFactory(0, 0x76, field1)
#define PIMP_TRANSACTION_AGREE_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_AGREED 0x77
#define PIMP_TRANSACTION_AGREED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x77, field1)
#define PIMP_TRANSACTION_AGREED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_BANK_REQUESTED 0x55
#define PIMP_TRANSACTION_BANK_REQUESTED_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0x55, field1, field2)
#define PIMP_TRANSACTION_BANK_REQUESTED_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_TRANSACTION_CANCEL 0x7E
#define PIMP_TRANSACTION_CANCEL_FACTORY(field1) MessageUnsignedIntFactory(0, 0x7E, field1)
#define PIMP_TRANSACTION_CANCEL_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_CANCELLED 0x7F
#define PIMP_TRANSACTION_CANCELLED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x7F, field1)
#define PIMP_TRANSACTION_CANCELLED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_CARD_ADDED 0x6B
#define PIMP_TRANSACTION_CARD_ADDED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x6B, field1, field2)
#define PIMP_TRANSACTION_CARD_ADDED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_CARD_REMOVED 0x6C
#define PIMP_TRANSACTION_CARD_REMOVED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x6C, field1, field2)
#define PIMP_TRANSACTION_CARD_REMOVED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_CARD_REQUESTED 0x53
#define PIMP_TRANSACTION_CARD_REQUESTED_FACTORY(field1, field2, field3, field4, field5) MessageUnsignedIntByteByteByteUnsignedIntFactory(0, 0x53, field1, field2, field3, field4, field5)
#define PIMP_TRANSACTION_CARD_REQUESTED_CAST(message, cast) MessageUnsignedIntByteByteByteUnsignedInt* cast = static_cast<MessageUnsignedIntByteByteByteUnsignedInt*> (message)
#define PIMP_TRANSACTION_CASH_SET 0x61
#define PIMP_TRANSACTION_CASH_SET_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0x61, field1, field2)
#define PIMP_TRANSACTION_CASH_SET_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_TRANSACTION_FINALISED 0x79
#define PIMP_TRANSACTION_FINALISED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x79, field1)
#define PIMP_TRANSACTION_FINALISED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_FINISH 0x70
#define PIMP_TRANSACTION_FINISH_FACTORY(field1) MessageUnsignedIntFactory(0, 0x70, field1)
#define PIMP_TRANSACTION_FINISH_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_FINISHED 0x71
#define PIMP_TRANSACTION_FINISHED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x71, field1)
#define PIMP_TRANSACTION_FINISHED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_JAIL_REQUESTED 0x56
#define PIMP_TRANSACTION_JAIL_REQUESTED_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0x56, field1, field2)
#define PIMP_TRANSACTION_JAIL_REQUESTED_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_TRANSACTION_OTHER_AGREED 0x78
#define PIMP_TRANSACTION_OTHER_AGREED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x78, field1)
#define PIMP_TRANSACTION_OTHER_AGREED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_OTHER_CARD_ADDED 0x6D
#define PIMP_TRANSACTION_OTHER_CARD_ADDED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x6D, field1, field2)
#define PIMP_TRANSACTION_OTHER_CARD_ADDED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_OTHER_CARD_REMOVED 0x6E
#define PIMP_TRANSACTION_OTHER_CARD_REMOVED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x6E, field1, field2)
#define PIMP_TRANSACTION_OTHER_CARD_REMOVED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_OTHER_CASH_SET 0x62
#define PIMP_TRANSACTION_OTHER_CASH_SET_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0x62, field1, field2)
#define PIMP_TRANSACTION_OTHER_CASH_SET_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_TRANSACTION_OTHER_FINISHED 0x72
#define PIMP_TRANSACTION_OTHER_FINISHED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x72, field1)
#define PIMP_TRANSACTION_OTHER_FINISHED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_OTHER_PROPERTY_ADDED 0x67
#define PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x67, field1, field2)
#define PIMP_TRANSACTION_OTHER_PROPERTY_ADDED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED 0x68
#define PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x68, field1, field2)
#define PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_OTHER_REOPENED 0x75
#define PIMP_TRANSACTION_OTHER_REOPENED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x75, field1)
#define PIMP_TRANSACTION_OTHER_REOPENED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_PROPERTY_ADDED 0x65
#define PIMP_TRANSACTION_PROPERTY_ADDED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x65, field1, field2)
#define PIMP_TRANSACTION_PROPERTY_ADDED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_PROPERTY_REMOVED 0x66
#define PIMP_TRANSACTION_PROPERTY_REMOVED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x66, field1, field2)
#define PIMP_TRANSACTION_PROPERTY_REMOVED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_REMOVE_CARD 0x6A
#define PIMP_TRANSACTION_REMOVE_CARD_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x6A, field1, field2)
#define PIMP_TRANSACTION_REMOVE_CARD_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_REMOVE_PROPERTY 0x64
#define PIMP_TRANSACTION_REMOVE_PROPERTY_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x64, field1, field2)
#define PIMP_TRANSACTION_REMOVE_PROPERTY_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_RENT_REQUESTED 0x52
#define PIMP_TRANSACTION_RENT_REQUESTED_FACTORY(field1, field2, field3, field4, field5) MessageUnsignedIntByteByteByteUnsignedIntFactory(0, 0x52, field1, field2, field3, field4, field5)
#define PIMP_TRANSACTION_RENT_REQUESTED_CAST(message, cast) MessageUnsignedIntByteByteByteUnsignedInt* cast = static_cast<MessageUnsignedIntByteByteByteUnsignedInt*> (message)
#define PIMP_TRANSACTION_REOPEN 0x73
#define PIMP_TRANSACTION_REOPEN_FACTORY(field1) MessageUnsignedIntFactory(0, 0x73, field1)
#define PIMP_TRANSACTION_REOPEN_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_REOPENED 0x74
#define PIMP_TRANSACTION_REOPENED_FACTORY(field1) MessageUnsignedIntFactory(0, 0x74, field1)
#define PIMP_TRANSACTION_REOPENED_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSACTION_REQUEST_TRADE 0x50
#define PIMP_TRANSACTION_REQUEST_TRADE_FACTORY(field1) MessageByteFactory(0, 0x50, field1)
#define PIMP_TRANSACTION_REQUEST_TRADE_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_TRANSACTION_SET_CASH 0x60
#define PIMP_TRANSACTION_SET_CASH_FACTORY(field1, field2) MessageUnsignedIntUnsignedIntFactory(0, 0x60, field1, field2)
#define PIMP_TRANSACTION_SET_CASH_CAST(message, cast) MessageUnsignedIntUnsignedInt* cast = static_cast<MessageUnsignedIntUnsignedInt*> (message)
#define PIMP_TRANSACTION_SQUARE_REQUESTED 0x54
#define PIMP_TRANSACTION_SQUARE_REQUESTED_FACTORY(field1, field2, field3) MessageUnsignedIntByteUnsignedIntFactory(0, 0x54, field1, field2, field3)
#define PIMP_TRANSACTION_SQUARE_REQUESTED_CAST(message, cast) MessageUnsignedIntByteUnsignedInt* cast = static_cast<MessageUnsignedIntByteUnsignedInt*> (message)
#define PIMP_TRANSACTION_TRADE_REQUESTED 0x51
#define PIMP_TRANSACTION_TRADE_REQUESTED_FACTORY(field1, field2) MessageUnsignedIntByteFactory(0, 0x51, field1, field2)
#define PIMP_TRANSACTION_TRADE_REQUESTED_CAST(message, cast) MessageUnsignedIntByte* cast = static_cast<MessageUnsignedIntByte*> (message)
#define PIMP_TRANSACTION_UNUSUAL 0x7D
#define PIMP_TRANSACTION_UNUSUAL_FACTORY(field1) MessageUnsignedIntFactory(0, 0x7D, field1)
#define PIMP_TRANSACTION_UNUSUAL_CAST(message, cast) MessageUnsignedInt* cast = static_cast<MessageUnsignedInt*> (message)
#define PIMP_TRANSFER 0xD4
#define PIMP_TRANSFER_FACTORY(field1) MessageByteFactory(0, 0xD4, field1)
#define PIMP_TRANSFER_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_UNMORTGAGE_PROPERTY 0x3C
#define PIMP_UNMORTGAGE_PROPERTY_FACTORY(field1) MessageByteFactory(0, 0x3C, field1)
#define PIMP_UNMORTGAGE_PROPERTY_CAST(message, cast) MessageByte* cast = static_cast<MessageByte*> (message)
#define PIMP_WAITING_FOR_TRANSACTION 0x2A
#define PIMP_WAITING_FOR_TRANSACTION_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0x2A, field1, field2)
#define PIMP_WAITING_FOR_TRANSACTION_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_WELCOME_BACK 0x0B
#define PIMP_WELCOME_BACK_FACTORY(field1, field2, field3, field4) MessageByteByteStringBoolFactory(0, 0x0B, field1, field2, field3, field4)
#define PIMP_WELCOME_BACK_CAST(message, cast) MessageByteByteStringBool* cast = static_cast<MessageByteByteStringBool*> (message)
#define PIMP_WELCOME_DETAILS 0x03
#define PIMP_WELCOME_DETAILS_FACTORY(field1, field2) MessageByteUnsignedIntFactory(0, 0x03, field1, field2)
#define PIMP_WELCOME_DETAILS_CAST(message, cast) MessageByteUnsignedInt* cast = static_cast<MessageByteUnsignedInt*> (message)
#define PIMP_WELCOME_OBSERVER 0x05
#define PIMP_WELCOME_OBSERVER_FACTORY(field1, field2, field3) MessageByteByteStringFactory(0, 0x05, field1, field2, field3)
#define PIMP_WELCOME_OBSERVER_CAST(message, cast) MessageByteByteString* cast = static_cast<MessageByteByteString*> (message)
#define PIMP_WELCOME_PLAYER 0x04
#define PIMP_WELCOME_PLAYER_FACTORY(field1, field2, field3) MessageByteByteStringFactory(0, 0x04, field1, field2, field3)
#define PIMP_WELCOME_PLAYER_CAST(message, cast) MessageByteByteString* cast = static_cast<MessageByteByteString*> (message)
