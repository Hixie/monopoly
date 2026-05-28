#ifndef INCLUDED_MESSAGE_H
#define INCLUDED_MESSAGE_H

/* PIMP: the Portable Internet Monopoly Protocol */

typedef unsigned char uint8;
typedef signed char int8;

#define MAX_PACKET_LENGTH 256
typedef uint8 MessageBuffer[MAX_PACKET_LENGTH];

#define MAX_STRING_LENGTH 32

#define MAX_USERS 255

extern bool DEBUG_MESSAGES;

class Message {
 public:
  Message(int player, uint8 type);
  Message(int player);
  Message(Message* message);
  virtual ~Message();
  virtual Message* Copy() { return new Message(this); }
  void Parse(MessageBuffer buffer, int length);
  int Serialize(MessageBuffer buffer); // returns the length of |buffer| that was used
  uint8 GetType() { return mType; }
  void SetType(uint8 type) { mType = type; }
  uint8 GetPlayer() { return mPlayer; }
  void SetPlayer(uint8 player) { mPlayer = player; }
 protected:
  virtual int ParsePayload(MessageBuffer buffer, int length);
  virtual uint8 SerializePayload(MessageBuffer buffer); // returns the length of the payload so far
 private:
  uint8 mType;
  int mPlayer;
};
Message* MessageFactory(int player, uint8 type);
Message* MessageDecoder(int player, MessageBuffer buffer, int length);

class MessageHouseChangeList : public Message {
 public:
  MessageHouseChangeList(int player, uint8 type, int count, int* properties, int* amountHouses, int* amountHotels);
  MessageHouseChangeList(int player);
  MessageHouseChangeList(MessageHouseChangeList* message);
  ~MessageHouseChangeList();
  Message* Copy() { return new MessageHouseChangeList(this); }
  uint8 GetField1() { return mCount; }
  uint8* GetField2() { return mProperties; }
  int8* GetField3() { return mAmountHouses; }
  int8* GetField4() { return mAmountHotels; }
 protected:
  int ParsePayload(MessageBuffer buffer, int length);
  uint8 SerializePayload(MessageBuffer buffer);
 private:
  uint8 mCount;
  uint8* mProperties;
  int8* mAmountHouses;
  int8* mAmountHotels;
};
Message* MessageHouseChangeListFactory(int player, uint8 type, int count, int* properties, int* amountHouses, int* amountHotels);
Message* MessageHouseChangeListDecoder(int player, MessageBuffer buffer, int length);

#include "message-factories.h.inc"

MessageFactoryPointer getMessageFactory(uint8 type);
char* getMessageName(uint8 type);

void DumpBuffer(uint8* buffer, int length);

#endif
