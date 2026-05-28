
#include <string.h>
#include <stdlib.h>
#include <netinet/in.h>
#include "exceptions.h"
#include "message.h"

bool DEBUG_MESSAGES = false;

union Convertor {
  bool Bool;
  uint8 Byte;
  int8 SignedByte;
  int Int;
  unsigned int UnsignedInt;
  struct {
    uint8 b1;
    uint8 b2;
    uint8 b3;
    uint8 b4;
  } FourBytes;
};

Message::Message(int player, uint8 type) :
  mType(type),
  mPlayer(player)
{ }

Message::Message(int player) :
  mPlayer(player)
{ }

Message::Message(Message* message) : 
  mType(message->GetType()),
  mPlayer(message->GetPlayer())
{ }

Message::~Message() {}

void Message::Parse(MessageBuffer buffer, int length)
{
  if (DEBUG_MESSAGES)
    cerr << "[";
  try {
    ParsePayload(buffer, length);
  } catch(...) {
    cerr << " error]" << endl;
    throw;
  }
  if (DEBUG_MESSAGES)
    cerr << "] (length " << (int)length << ")" << endl;
}

int Message::Serialize(MessageBuffer buffer) {
  if (DEBUG_MESSAGES) {
    cerr << "[";
    cerr << getMessageName(mType) << " (0x" << hex << (int)mType << "): ";
  }
  buffer[0] = mType;
  uint8 length = SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "] (length " << (int)length << ")" << endl;
  buffer[1] = length - 2;
  return length;
}

int Message::ParsePayload(MessageBuffer buffer, int length)
{
  if (length >= 2) {
    mType = buffer[0];
    if (DEBUG_MESSAGES)
      cerr << getMessageName(mType) << " (0x" << hex << (int)mType << "): ";
    // ignore the length byte, it's handled by the caller
    return 2;
  } else {
    throw ParseError();
  }
}

uint8 Message::SerializePayload(MessageBuffer buffer) {
  return 2; // the subclass should start at index 2 (after the header)
}

Message* MessageFactory(int player, uint8 type) {
  return new Message(player, type);
};

Message* MessageDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new Message(player);
  message->Parse(buffer, length);
  return message;
};


MessageHouseChangeList::MessageHouseChangeList(int player, uint8 type, int count, int* properties, int* amountHouses, int* amountHotels) :
  Message(player, type),
  mCount(count)
{
  if (mCount) {
    mProperties = new uint8[mCount];
    mAmountHouses = new int8[mCount];
    mAmountHotels = new int8[mCount];
    for (int i = 0; i < mCount; ++i) {
      mProperties[i] = properties[i];
      mAmountHouses[i] = amountHouses[i];
      mAmountHotels[i] = amountHotels[i];
    }
  } else {
    mProperties = NULL;
    mAmountHouses = NULL;
    mAmountHotels = NULL;
  }  
}

MessageHouseChangeList::MessageHouseChangeList(int player) :
  Message(player),
  mCount(0),
  mProperties(NULL),
  mAmountHouses(NULL),
  mAmountHotels(NULL)
{ }

MessageHouseChangeList::MessageHouseChangeList(MessageHouseChangeList* message) :
  Message(message),
  mCount(message->mCount)
{
  if (mCount) {
    mProperties = new uint8[mCount];
    mAmountHouses = new int8[mCount];
    mAmountHotels = new int8[mCount];
    for (int i = 0; i < mCount; ++i) {
      mProperties[i] = message->mProperties[i];
      mAmountHouses[i] = message->mAmountHouses[i];
      mAmountHotels[i] = message->mAmountHotels[i];
    }
  } else {
    mProperties = NULL;
    mAmountHouses = NULL;
    mAmountHotels = NULL;
  }  
}

MessageHouseChangeList::~MessageHouseChangeList() {
  if (mCount) {
    delete[] mProperties;
    delete[] mAmountHouses;
    delete[] mAmountHotels;
  }
}

int MessageHouseChangeList::ParsePayload(MessageBuffer buffer, int length) {
  int index = Message::ParsePayload(buffer, length);
  if (length - index >= 1) {
    if (mCount) {
      delete[] mProperties;
      delete[] mAmountHouses;
      delete[] mAmountHotels;
    }
    // first byte is the count
    mCount = buffer[index++];
    if (DEBUG_MESSAGES)
      cerr << "count = " << dec << (int)mCount << ": ";
    if (mCount) {
      if (index + mCount * 3 > length) {
        mCount = 0;
        throw ParseError();
      }
      mProperties = new uint8[mCount];
      mAmountHouses = new int8[mCount];
      mAmountHotels = new int8[mCount];
      for (int i = 0; i < mCount; ++i) {
        mProperties[i] = buffer[index++];
        Convertor value;
        value.Byte = buffer[index++];
        mAmountHouses[i] = value.SignedByte;
        value.Byte = buffer[index++];
        mAmountHotels[i] = value.SignedByte;
        if (DEBUG_MESSAGES)
          cerr << "(" << (int)mProperties[i]
               << "," << (int)mAmountHouses[i]
               << "," << (int)mAmountHotels[i] << ")";
      }
    }
    return index;
  } else {
    throw ParseError();
  }
}

uint8 MessageHouseChangeList::SerializePayload(MessageBuffer buffer) {
  uint8 length = Message::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(...)";
  buffer[length++] = mCount;
  for (int i = 0; i < mCount; ++i) {
    buffer[length++] = mProperties[i];
    Convertor value;
    value.SignedByte = mAmountHouses[i];
    buffer[length++] = value.Byte;
    value.SignedByte = mAmountHotels[i];
    buffer[length++] = value.Byte;
  }
  return length;
}

Message* MessageHouseChangeListFactory(int player, uint8 type, int count, int* properties, int* amountHouses, int* amountHotels) {
  return new MessageHouseChangeList(player, type, count, properties, amountHouses, amountHotels);
};

Message* MessageHouseChangeListDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageHouseChangeList(player);
  message->Parse(buffer, length);
  return message;
};

#include "message-factories.cpp.inc"

int getMessagePosition(uint8 type) {
  // do a binary tree search of the array defined at the bottom of message-factories.cpp.inc
  uint8 low = 0;
  uint8 high = NUM_MESSAGE_FACTORIES - 1;
  for (;;) {
    uint8 index = (low + high) / 2; // middle index
    uint8 value = messageFactories[index].type;
    if (type > value) {
      // we want the right hand side
      low = index + 1;
    } else if (type < value) {
      // we want the left hand side
      high = index - 1;
    } else {
      // this is it (must be, they are equal)
      return index;
    }
    if (low > high) {
      // not there
      return -1;
    }
  }
}

MessageFactoryPointer getMessageFactory(uint8 type) {
  int index = getMessagePosition(type);
  if (index < 0)
    return NULL;
  return messageFactories[index].factory;
}

char* getMessageName(uint8 type) {
  int index = getMessagePosition(type);
  if (index < 0)
    return "";
  return messageFactories[index].name;
}

void DumpBuffer(uint8* buffer, int length) {
  int index = 0;
  ios::fmtflags flags = cerr.flags();
  cerr << hex << "  ";
  while (index < length) {
    int byte = buffer[index++];
    if (byte < 0x10) {
      cerr << "0";
    }
    cerr << byte << " ";
    if (index % 16 == 0) {
      cerr << endl << "  ";
    } else if (index % 8 == 0) {
      cerr << " ";
    }
  }
  cerr << endl;
  cerr.setf(flags);
}
