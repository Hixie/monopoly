 // THIS FILE IS AUTO-GENERATED: DO NOT EDIT

MessageByte::MessageByte(int player, uint8 type, uint8 field1) :
  Message(player, type),
  mField1(field1)
{ }

MessageByte::MessageByte(int player) :
  Message(player),
  mField1(0)
{ }

MessageByte::MessageByte(MessageByte* message) :
  Message(message),
  mField1(message->GetField1())
{ }

int MessageByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = Message::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField1 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField1 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = Message::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField1 << ")";
  buffer[length] = mField1;
  return length + 1;
}

Message* MessageByteFactory(int player, uint8 type, uint8 field1) {
  return new MessageByte(player, type, field1);
};

Message* MessageByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedInt::MessageUnsignedInt(int player, uint8 type, unsigned int field1) :
  Message(player, type),
  mField1(field1)
{ }

MessageUnsignedInt::MessageUnsignedInt(int player) :
  Message(player),
  mField1(0)
{ }

MessageUnsignedInt::MessageUnsignedInt(MessageUnsignedInt* message) :
  Message(message),
  mField1(message->GetField1())
{ }

int MessageUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = Message::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField1 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField1 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = Message::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField1 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField1);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageUnsignedIntFactory(int player, uint8 type, unsigned int field1) {
  return new MessageUnsignedInt(player, type, field1);
};

Message* MessageUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteBool::MessageByteBool(int player, uint8 type, uint8 field1, bool field2) :
  MessageByte(player, type, field1),
  mField2(field2)
{ }

MessageByteBool::MessageByteBool(int player) :
  MessageByte(player),
  mField2(0)
{ }

MessageByteBool::MessageByteBool(MessageByteBool* message) :
  MessageByte(message),
  mField2(message->GetField2())
{ }

int MessageByteBool::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    Convertor data;
    data.Byte = buffer[index];
    mField2 = data.Bool;
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField2 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteBool::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField2 << ")";
  Convertor data;
  data.Bool = mField2;
  buffer[length] = data.Byte;
  return length + 1;
}

Message* MessageByteBoolFactory(int player, uint8 type, uint8 field1, bool field2) {
  return new MessageByteBool(player, type, field1, field2);
};

Message* MessageByteBoolDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteBool(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByte::MessageByteByte(int player, uint8 type, uint8 field1, uint8 field2) :
  MessageByte(player, type, field1),
  mField2(field2)
{ }

MessageByteByte::MessageByteByte(int player) :
  MessageByte(player),
  mField2(0)
{ }

MessageByteByte::MessageByteByte(MessageByteByte* message) :
  MessageByte(message),
  mField2(message->GetField2())
{ }

int MessageByteByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField2 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField2 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField2 << ")";
  buffer[length] = mField2;
  return length + 1;
}

Message* MessageByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2) {
  return new MessageByteByte(player, type, field1, field2);
};

Message* MessageByteByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteUnsignedInt::MessageByteUnsignedInt(int player, uint8 type, uint8 field1, unsigned int field2) :
  MessageByte(player, type, field1),
  mField2(field2)
{ }

MessageByteUnsignedInt::MessageByteUnsignedInt(int player) :
  MessageByte(player),
  mField2(0)
{ }

MessageByteUnsignedInt::MessageByteUnsignedInt(MessageByteUnsignedInt* message) :
  MessageByte(message),
  mField2(message->GetField2())
{ }

int MessageByteUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByte::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField2 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField2 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField2 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField2);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageByteUnsignedIntFactory(int player, uint8 type, uint8 field1, unsigned int field2) {
  return new MessageByteUnsignedInt(player, type, field1, field2);
};

Message* MessageByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntByte::MessageUnsignedIntByte(int player, uint8 type, unsigned int field1, uint8 field2) :
  MessageUnsignedInt(player, type, field1),
  mField2(field2)
{ }

MessageUnsignedIntByte::MessageUnsignedIntByte(int player) :
  MessageUnsignedInt(player),
  mField2(0)
{ }

MessageUnsignedIntByte::MessageUnsignedIntByte(MessageUnsignedIntByte* message) :
  MessageUnsignedInt(message),
  mField2(message->GetField2())
{ }

int MessageUnsignedIntByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedInt::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField2 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField2 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedInt::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField2 << ")";
  buffer[length] = mField2;
  return length + 1;
}

Message* MessageUnsignedIntByteFactory(int player, uint8 type, unsigned int field1, uint8 field2) {
  return new MessageUnsignedIntByte(player, type, field1, field2);
};

Message* MessageUnsignedIntByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntString::MessageUnsignedIntString(int player, uint8 type, unsigned int field1, char* field2) :
  MessageUnsignedInt(player, type, field1),
  mField2(strdup(field2))
{ }

MessageUnsignedIntString::MessageUnsignedIntString(int player) :
  MessageUnsignedInt(player),
  mField2(NULL)
{ }

MessageUnsignedIntString::MessageUnsignedIntString(MessageUnsignedIntString* message) :
  MessageUnsignedInt(message),
  mField2(message->GetField2())
{ }


MessageUnsignedIntString::~MessageUnsignedIntString() {
  free(mField2);
}

int MessageUnsignedIntString::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedInt::ParsePayload(buffer, length);
  mField2 = (char*)(malloc(MAX_STRING_LENGTH+1));
  if (length - index >= 1) {
    // first byte is the string length
    int size = buffer[index++];
    if (index + size > length) 
      throw ParseError();
    if (size > MAX_STRING_LENGTH) 
      throw ParseError();
    for (int i = 0; i < size; ++i) {
      mField2[i] = buffer[i+index];
    }
    mField2[size] = '\0';
    if (DEBUG_MESSAGES)
      cerr << "(" << mField2 << ")";
    return index+size;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntString::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedInt::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << mField2 << ")";
  int size = strlen(mField2);
  buffer[length] = size;
  for (int i = 0; i < size; ++i) {
    buffer[length+i+1] = mField2[i];
  }
  return length+size+1;
}

Message* MessageUnsignedIntStringFactory(int player, uint8 type, unsigned int field1, char* field2) {
  return new MessageUnsignedIntString(player, type, field1, field2);
};

Message* MessageUnsignedIntStringDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntString(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntUnsignedInt::MessageUnsignedIntUnsignedInt(int player, uint8 type, unsigned int field1, unsigned int field2) :
  MessageUnsignedInt(player, type, field1),
  mField2(field2)
{ }

MessageUnsignedIntUnsignedInt::MessageUnsignedIntUnsignedInt(int player) :
  MessageUnsignedInt(player),
  mField2(0)
{ }

MessageUnsignedIntUnsignedInt::MessageUnsignedIntUnsignedInt(MessageUnsignedIntUnsignedInt* message) :
  MessageUnsignedInt(message),
  mField2(message->GetField2())
{ }

int MessageUnsignedIntUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedInt::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField2 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField2 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedInt::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField2 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField2);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageUnsignedIntUnsignedIntFactory(int player, uint8 type, unsigned int field1, unsigned int field2) {
  return new MessageUnsignedIntUnsignedInt(player, type, field1, field2);
};

Message* MessageUnsignedIntUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteBoolString::MessageByteBoolString(int player, uint8 type, uint8 field1, bool field2, char* field3) :
  MessageByteBool(player, type, field1, field2),
  mField3(strdup(field3))
{ }

MessageByteBoolString::MessageByteBoolString(int player) :
  MessageByteBool(player),
  mField3(NULL)
{ }

MessageByteBoolString::MessageByteBoolString(MessageByteBoolString* message) :
  MessageByteBool(message),
  mField3(message->GetField3())
{ }


MessageByteBoolString::~MessageByteBoolString() {
  free(mField3);
}

int MessageByteBoolString::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteBool::ParsePayload(buffer, length);
  mField3 = (char*)(malloc(MAX_STRING_LENGTH+1));
  if (length - index >= 1) {
    // first byte is the string length
    int size = buffer[index++];
    if (index + size > length) 
      throw ParseError();
    if (size > MAX_STRING_LENGTH) 
      throw ParseError();
    for (int i = 0; i < size; ++i) {
      mField3[i] = buffer[i+index];
    }
    mField3[size] = '\0';
    if (DEBUG_MESSAGES)
      cerr << "(" << mField3 << ")";
    return index+size;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteBoolString::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteBool::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << mField3 << ")";
  int size = strlen(mField3);
  buffer[length] = size;
  for (int i = 0; i < size; ++i) {
    buffer[length+i+1] = mField3[i];
  }
  return length+size+1;
}

Message* MessageByteBoolStringFactory(int player, uint8 type, uint8 field1, bool field2, char* field3) {
  return new MessageByteBoolString(player, type, field1, field2, field3);
};

Message* MessageByteBoolStringDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteBoolString(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteBool::MessageByteByteBool(int player, uint8 type, uint8 field1, uint8 field2, bool field3) :
  MessageByteByte(player, type, field1, field2),
  mField3(field3)
{ }

MessageByteByteBool::MessageByteByteBool(int player) :
  MessageByteByte(player),
  mField3(0)
{ }

MessageByteByteBool::MessageByteByteBool(MessageByteByteBool* message) :
  MessageByteByte(message),
  mField3(message->GetField3())
{ }

int MessageByteByteBool::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    Convertor data;
    data.Byte = buffer[index];
    mField3 = data.Bool;
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField3 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteBool::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField3 << ")";
  Convertor data;
  data.Bool = mField3;
  buffer[length] = data.Byte;
  return length + 1;
}

Message* MessageByteByteBoolFactory(int player, uint8 type, uint8 field1, uint8 field2, bool field3) {
  return new MessageByteByteBool(player, type, field1, field2, field3);
};

Message* MessageByteByteBoolDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteBool(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteByte::MessageByteByteByte(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3) :
  MessageByteByte(player, type, field1, field2),
  mField3(field3)
{ }

MessageByteByteByte::MessageByteByteByte(int player) :
  MessageByteByte(player),
  mField3(0)
{ }

MessageByteByteByte::MessageByteByteByte(MessageByteByteByte* message) :
  MessageByteByte(message),
  mField3(message->GetField3())
{ }

int MessageByteByteByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField3 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField3 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField3 << ")";
  buffer[length] = mField3;
  return length + 1;
}

Message* MessageByteByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3) {
  return new MessageByteByteByte(player, type, field1, field2, field3);
};

Message* MessageByteByteByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteString::MessageByteByteString(int player, uint8 type, uint8 field1, uint8 field2, char* field3) :
  MessageByteByte(player, type, field1, field2),
  mField3(strdup(field3))
{ }

MessageByteByteString::MessageByteByteString(int player) :
  MessageByteByte(player),
  mField3(NULL)
{ }

MessageByteByteString::MessageByteByteString(MessageByteByteString* message) :
  MessageByteByte(message),
  mField3(message->GetField3())
{ }


MessageByteByteString::~MessageByteByteString() {
  free(mField3);
}

int MessageByteByteString::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByte::ParsePayload(buffer, length);
  mField3 = (char*)(malloc(MAX_STRING_LENGTH+1));
  if (length - index >= 1) {
    // first byte is the string length
    int size = buffer[index++];
    if (index + size > length) 
      throw ParseError();
    if (size > MAX_STRING_LENGTH) 
      throw ParseError();
    for (int i = 0; i < size; ++i) {
      mField3[i] = buffer[i+index];
    }
    mField3[size] = '\0';
    if (DEBUG_MESSAGES)
      cerr << "(" << mField3 << ")";
    return index+size;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteString::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << mField3 << ")";
  int size = strlen(mField3);
  buffer[length] = size;
  for (int i = 0; i < size; ++i) {
    buffer[length+i+1] = mField3[i];
  }
  return length+size+1;
}

Message* MessageByteByteStringFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3) {
  return new MessageByteByteString(player, type, field1, field2, field3);
};

Message* MessageByteByteStringDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteString(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteUnsignedInt::MessageByteByteUnsignedInt(int player, uint8 type, uint8 field1, uint8 field2, unsigned int field3) :
  MessageByteByte(player, type, field1, field2),
  mField3(field3)
{ }

MessageByteByteUnsignedInt::MessageByteByteUnsignedInt(int player) :
  MessageByteByte(player),
  mField3(0)
{ }

MessageByteByteUnsignedInt::MessageByteByteUnsignedInt(MessageByteByteUnsignedInt* message) :
  MessageByteByte(message),
  mField3(message->GetField3())
{ }

int MessageByteByteUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByte::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField3 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField3 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField3 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField3);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageByteByteUnsignedIntFactory(int player, uint8 type, uint8 field1, uint8 field2, unsigned int field3) {
  return new MessageByteByteUnsignedInt(player, type, field1, field2, field3);
};

Message* MessageByteByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteUnsignedIntUnsignedInt::MessageByteUnsignedIntUnsignedInt(int player, uint8 type, uint8 field1, unsigned int field2, unsigned int field3) :
  MessageByteUnsignedInt(player, type, field1, field2),
  mField3(field3)
{ }

MessageByteUnsignedIntUnsignedInt::MessageByteUnsignedIntUnsignedInt(int player) :
  MessageByteUnsignedInt(player),
  mField3(0)
{ }

MessageByteUnsignedIntUnsignedInt::MessageByteUnsignedIntUnsignedInt(MessageByteUnsignedIntUnsignedInt* message) :
  MessageByteUnsignedInt(message),
  mField3(message->GetField3())
{ }

int MessageByteUnsignedIntUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteUnsignedInt::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField3 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField3 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteUnsignedIntUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteUnsignedInt::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField3 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField3);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageByteUnsignedIntUnsignedIntFactory(int player, uint8 type, uint8 field1, unsigned int field2, unsigned int field3) {
  return new MessageByteUnsignedIntUnsignedInt(player, type, field1, field2, field3);
};

Message* MessageByteUnsignedIntUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteUnsignedIntUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntByteByte::MessageUnsignedIntByteByte(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3) :
  MessageUnsignedIntByte(player, type, field1, field2),
  mField3(field3)
{ }

MessageUnsignedIntByteByte::MessageUnsignedIntByteByte(int player) :
  MessageUnsignedIntByte(player),
  mField3(0)
{ }

MessageUnsignedIntByteByte::MessageUnsignedIntByteByte(MessageUnsignedIntByteByte* message) :
  MessageUnsignedIntByte(message),
  mField3(message->GetField3())
{ }

int MessageUnsignedIntByteByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedIntByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField3 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField3 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntByteByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedIntByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField3 << ")";
  buffer[length] = mField3;
  return length + 1;
}

Message* MessageUnsignedIntByteByteFactory(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3) {
  return new MessageUnsignedIntByteByte(player, type, field1, field2, field3);
};

Message* MessageUnsignedIntByteByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntByteByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntByteUnsignedInt::MessageUnsignedIntByteUnsignedInt(int player, uint8 type, unsigned int field1, uint8 field2, unsigned int field3) :
  MessageUnsignedIntByte(player, type, field1, field2),
  mField3(field3)
{ }

MessageUnsignedIntByteUnsignedInt::MessageUnsignedIntByteUnsignedInt(int player) :
  MessageUnsignedIntByte(player),
  mField3(0)
{ }

MessageUnsignedIntByteUnsignedInt::MessageUnsignedIntByteUnsignedInt(MessageUnsignedIntByteUnsignedInt* message) :
  MessageUnsignedIntByte(message),
  mField3(message->GetField3())
{ }

int MessageUnsignedIntByteUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedIntByte::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField3 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField3 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntByteUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedIntByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField3 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField3);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageUnsignedIntByteUnsignedIntFactory(int player, uint8 type, unsigned int field1, uint8 field2, unsigned int field3) {
  return new MessageUnsignedIntByteUnsignedInt(player, type, field1, field2, field3);
};

Message* MessageUnsignedIntByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntByteUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteBoolByte::MessageByteByteBoolByte(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4) :
  MessageByteByteBool(player, type, field1, field2, field3),
  mField4(field4)
{ }

MessageByteByteBoolByte::MessageByteByteBoolByte(int player) :
  MessageByteByteBool(player),
  mField4(0)
{ }

MessageByteByteBoolByte::MessageByteByteBoolByte(MessageByteByteBoolByte* message) :
  MessageByteByteBool(message),
  mField4(message->GetField4())
{ }

int MessageByteByteBoolByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteBool::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField4 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField4 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteBoolByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteBool::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField4 << ")";
  buffer[length] = mField4;
  return length + 1;
}

Message* MessageByteByteBoolByteFactory(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4) {
  return new MessageByteByteBoolByte(player, type, field1, field2, field3, field4);
};

Message* MessageByteByteBoolByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteBoolByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteByteByte::MessageByteByteByteByte(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, uint8 field4) :
  MessageByteByteByte(player, type, field1, field2, field3),
  mField4(field4)
{ }

MessageByteByteByteByte::MessageByteByteByteByte(int player) :
  MessageByteByteByte(player),
  mField4(0)
{ }

MessageByteByteByteByte::MessageByteByteByteByte(MessageByteByteByteByte* message) :
  MessageByteByteByte(message),
  mField4(message->GetField4())
{ }

int MessageByteByteByteByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField4 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField4 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteByteByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField4 << ")";
  buffer[length] = mField4;
  return length + 1;
}

Message* MessageByteByteByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, uint8 field4) {
  return new MessageByteByteByteByte(player, type, field1, field2, field3, field4);
};

Message* MessageByteByteByteByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteByteByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteByteUnsignedInt::MessageByteByteByteUnsignedInt(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, unsigned int field4) :
  MessageByteByteByte(player, type, field1, field2, field3),
  mField4(field4)
{ }

MessageByteByteByteUnsignedInt::MessageByteByteByteUnsignedInt(int player) :
  MessageByteByteByte(player),
  mField4(0)
{ }

MessageByteByteByteUnsignedInt::MessageByteByteByteUnsignedInt(MessageByteByteByteUnsignedInt* message) :
  MessageByteByteByte(message),
  mField4(message->GetField4())
{ }

int MessageByteByteByteUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteByte::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField4 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField4 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteByteUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField4 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField4);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageByteByteByteUnsignedIntFactory(int player, uint8 type, uint8 field1, uint8 field2, uint8 field3, unsigned int field4) {
  return new MessageByteByteByteUnsignedInt(player, type, field1, field2, field3, field4);
};

Message* MessageByteByteByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteByteUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteStringBool::MessageByteByteStringBool(int player, uint8 type, uint8 field1, uint8 field2, char* field3, bool field4) :
  MessageByteByteString(player, type, field1, field2, field3),
  mField4(field4)
{ }

MessageByteByteStringBool::MessageByteByteStringBool(int player) :
  MessageByteByteString(player),
  mField4(0)
{ }

MessageByteByteStringBool::MessageByteByteStringBool(MessageByteByteStringBool* message) :
  MessageByteByteString(message),
  mField4(message->GetField4())
{ }

int MessageByteByteStringBool::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteString::ParsePayload(buffer, length);
  if (length - index >= 1) {
    Convertor data;
    data.Byte = buffer[index];
    mField4 = data.Bool;
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField4 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteStringBool::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteString::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField4 << ")";
  Convertor data;
  data.Bool = mField4;
  buffer[length] = data.Byte;
  return length + 1;
}

Message* MessageByteByteStringBoolFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, bool field4) {
  return new MessageByteByteStringBool(player, type, field1, field2, field3, field4);
};

Message* MessageByteByteStringBoolDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteStringBool(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteStringByte::MessageByteByteStringByte(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4) :
  MessageByteByteString(player, type, field1, field2, field3),
  mField4(field4)
{ }

MessageByteByteStringByte::MessageByteByteStringByte(int player) :
  MessageByteByteString(player),
  mField4(0)
{ }

MessageByteByteStringByte::MessageByteByteStringByte(MessageByteByteStringByte* message) :
  MessageByteByteString(message),
  mField4(message->GetField4())
{ }

int MessageByteByteStringByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteString::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField4 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField4 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteStringByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteString::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField4 << ")";
  buffer[length] = mField4;
  return length + 1;
}

Message* MessageByteByteStringByteFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4) {
  return new MessageByteByteStringByte(player, type, field1, field2, field3, field4);
};

Message* MessageByteByteStringByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteStringByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntByteByteByte::MessageUnsignedIntByteByteByte(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4) :
  MessageUnsignedIntByteByte(player, type, field1, field2, field3),
  mField4(field4)
{ }

MessageUnsignedIntByteByteByte::MessageUnsignedIntByteByteByte(int player) :
  MessageUnsignedIntByteByte(player),
  mField4(0)
{ }

MessageUnsignedIntByteByteByte::MessageUnsignedIntByteByteByte(MessageUnsignedIntByteByteByte* message) :
  MessageUnsignedIntByteByte(message),
  mField4(message->GetField4())
{ }

int MessageUnsignedIntByteByteByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedIntByteByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField4 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField4 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntByteByteByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedIntByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField4 << ")";
  buffer[length] = mField4;
  return length + 1;
}

Message* MessageUnsignedIntByteByteByteFactory(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4) {
  return new MessageUnsignedIntByteByteByte(player, type, field1, field2, field3, field4);
};

Message* MessageUnsignedIntByteByteByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntByteByteByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteBoolByteByte::MessageByteByteBoolByteByte(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4, uint8 field5) :
  MessageByteByteBoolByte(player, type, field1, field2, field3, field4),
  mField5(field5)
{ }

MessageByteByteBoolByteByte::MessageByteByteBoolByteByte(int player) :
  MessageByteByteBoolByte(player),
  mField5(0)
{ }

MessageByteByteBoolByteByte::MessageByteByteBoolByteByte(MessageByteByteBoolByteByte* message) :
  MessageByteByteBoolByte(message),
  mField5(message->GetField5())
{ }

int MessageByteByteBoolByteByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteBoolByte::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField5 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField5 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteBoolByteByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteBoolByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField5 << ")";
  buffer[length] = mField5;
  return length + 1;
}

Message* MessageByteByteBoolByteByteFactory(int player, uint8 type, uint8 field1, uint8 field2, bool field3, uint8 field4, uint8 field5) {
  return new MessageByteByteBoolByteByte(player, type, field1, field2, field3, field4, field5);
};

Message* MessageByteByteBoolByteByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteBoolByteByte(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteStringByteUnsignedInt::MessageByteByteStringByteUnsignedInt(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5) :
  MessageByteByteStringByte(player, type, field1, field2, field3, field4),
  mField5(field5)
{ }

MessageByteByteStringByteUnsignedInt::MessageByteByteStringByteUnsignedInt(int player) :
  MessageByteByteStringByte(player),
  mField5(0)
{ }

MessageByteByteStringByteUnsignedInt::MessageByteByteStringByteUnsignedInt(MessageByteByteStringByteUnsignedInt* message) :
  MessageByteByteStringByte(message),
  mField5(message->GetField5())
{ }

int MessageByteByteStringByteUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteStringByte::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField5 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField5 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteStringByteUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteStringByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField5 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField5);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageByteByteStringByteUnsignedIntFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5) {
  return new MessageByteByteStringByteUnsignedInt(player, type, field1, field2, field3, field4, field5);
};

Message* MessageByteByteStringByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteStringByteUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageUnsignedIntByteByteByteUnsignedInt::MessageUnsignedIntByteByteByteUnsignedInt(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4, unsigned int field5) :
  MessageUnsignedIntByteByteByte(player, type, field1, field2, field3, field4),
  mField5(field5)
{ }

MessageUnsignedIntByteByteByteUnsignedInt::MessageUnsignedIntByteByteByteUnsignedInt(int player) :
  MessageUnsignedIntByteByteByte(player),
  mField5(0)
{ }

MessageUnsignedIntByteByteByteUnsignedInt::MessageUnsignedIntByteByteByteUnsignedInt(MessageUnsignedIntByteByteByteUnsignedInt* message) :
  MessageUnsignedIntByteByteByte(message),
  mField5(message->GetField5())
{ }

int MessageUnsignedIntByteByteByteUnsignedInt::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageUnsignedIntByteByteByte::ParsePayload(buffer, length);
  if (length - index >= 4) {
    Convertor data;
    data.FourBytes.b1 = buffer[index+0];
    data.FourBytes.b2 = buffer[index+1];
    data.FourBytes.b3 = buffer[index+2];
    data.FourBytes.b4 = buffer[index+3];
    mField5 = ntohl(data.UnsignedInt);
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField5 << ")";
    return index + 4;
  } else {
    throw ParseError();
  }
}

uint8 MessageUnsignedIntByteByteByteUnsignedInt::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageUnsignedIntByteByteByte::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField5 << ")";
  Convertor data;
  data.UnsignedInt = htonl(mField5);
  buffer[length+0] = data.FourBytes.b1;
  buffer[length+1] = data.FourBytes.b2;
  buffer[length+2] = data.FourBytes.b3;
  buffer[length+3] = data.FourBytes.b4;
  return length + 4;
}

Message* MessageUnsignedIntByteByteByteUnsignedIntFactory(int player, uint8 type, unsigned int field1, uint8 field2, uint8 field3, uint8 field4, unsigned int field5) {
  return new MessageUnsignedIntByteByteByteUnsignedInt(player, type, field1, field2, field3, field4, field5);
};

Message* MessageUnsignedIntByteByteByteUnsignedIntDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageUnsignedIntByteByteByteUnsignedInt(player);
  message->Parse(buffer, length);
  return message;
};


MessageByteByteStringByteUnsignedIntByte::MessageByteByteStringByteUnsignedIntByte(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5, uint8 field6) :
  MessageByteByteStringByteUnsignedInt(player, type, field1, field2, field3, field4, field5),
  mField6(field6)
{ }

MessageByteByteStringByteUnsignedIntByte::MessageByteByteStringByteUnsignedIntByte(int player) :
  MessageByteByteStringByteUnsignedInt(player),
  mField6(0)
{ }

MessageByteByteStringByteUnsignedIntByte::MessageByteByteStringByteUnsignedIntByte(MessageByteByteStringByteUnsignedIntByte* message) :
  MessageByteByteStringByteUnsignedInt(message),
  mField6(message->GetField6())
{ }

int MessageByteByteStringByteUnsignedIntByte::ParsePayload(MessageBuffer buffer, int length) {
  int index = MessageByteByteStringByteUnsignedInt::ParsePayload(buffer, length);
  if (length - index >= 1) {
    mField6 = buffer[index];
    if (DEBUG_MESSAGES)
      cerr << "(" << dec << (int)mField6 << ")";
    return index + 1;
  } else {
    throw ParseError();
  }
}

uint8 MessageByteByteStringByteUnsignedIntByte::SerializePayload(MessageBuffer buffer) {
  uint8 length = MessageByteByteStringByteUnsignedInt::SerializePayload(buffer);
  if (DEBUG_MESSAGES)
    cerr << "(" << dec << (int)mField6 << ")";
  buffer[length] = mField6;
  return length + 1;
}

Message* MessageByteByteStringByteUnsignedIntByteFactory(int player, uint8 type, uint8 field1, uint8 field2, char* field3, uint8 field4, unsigned int field5, uint8 field6) {
  return new MessageByteByteStringByteUnsignedIntByte(player, type, field1, field2, field3, field4, field5, field6);
};

Message* MessageByteByteStringByteUnsignedIntByteDecoder(int player, MessageBuffer buffer, int length) {
  Message* message = new MessageByteByteStringByteUnsignedIntByte(player);
  message->Parse(buffer, length);
  return message;
};


#define NUM_MESSAGE_FACTORIES 159
struct MessageFactoryMap {
  uint8 type;
  MessageFactoryPointer factory;
  char* name;
} messageFactories[NUM_MESSAGE_FACTORIES] = {
  // This absolutely positively must be in ascending numerical order (it is binary tree searched)
  { PIMP_HANDSHAKE, &MessageByteDecoder, "PIMP_HANDSHAKE" },
  { PIMP_HANDSHAKE_ACKNOWLEDGE, &MessageUnsignedIntDecoder, "PIMP_HANDSHAKE_ACKNOWLEDGE" },
  { PIMP_JOIN, &MessageByteBoolStringDecoder, "PIMP_JOIN" },
  { PIMP_WELCOME_DETAILS, &MessageByteUnsignedIntDecoder, "PIMP_WELCOME_DETAILS" },
  { PIMP_WELCOME_PLAYER, &MessageByteByteStringDecoder, "PIMP_WELCOME_PLAYER" },
  { PIMP_WELCOME_OBSERVER, &MessageByteByteStringDecoder, "PIMP_WELCOME_OBSERVER" },
  { PIMP_QUERY_JOIN_PLAY, &MessageUnsignedIntStringDecoder, "PIMP_QUERY_JOIN_PLAY" },
  { PIMP_QUERY_JOIN_OBSERVE, &MessageUnsignedIntStringDecoder, "PIMP_QUERY_JOIN_OBSERVE" },
  { PIMP_ACCEPT_JOIN, &MessageUnsignedIntDecoder, "PIMP_ACCEPT_JOIN" },
  { PIMP_REFUSE_JOIN, &MessageUnsignedIntDecoder, "PIMP_REFUSE_JOIN" },
  { PIMP_REJOIN, &MessageByteUnsignedIntDecoder, "PIMP_REJOIN" },
  { PIMP_WELCOME_BACK, &MessageByteByteStringBoolDecoder, "PIMP_WELCOME_BACK" },
  { PIMP_PLAYER_TAKEOVER, &MessageByteByteDecoder, "PIMP_PLAYER_TAKEOVER" },
  { PIMP_SWITCH_PLAY, &MessageDecoder, "PIMP_SWITCH_PLAY" },
  { PIMP_OBSERVER_BECAME_PLAYER, &MessageByteDecoder, "PIMP_OBSERVER_BECAME_PLAYER" },
  { PIMP_JOIN_REFUSED, &MessageUnsignedIntStringDecoder, "PIMP_JOIN_REFUSED" },
  { PIMP_JOIN_PENDING, &MessageDecoder, "PIMP_JOIN_PENDING" },
  { PIMP_REQUEST_STATE, &MessageDecoder, "PIMP_REQUEST_STATE" },
  { PIMP_STATE_BOARD, &MessageByteDecoder, "PIMP_STATE_BOARD" },
  { PIMP_STATE_PLAYER, &MessageByteByteStringByteUnsignedIntByteDecoder, "PIMP_STATE_PLAYER" },
  { PIMP_STATE_OBSERVER, &MessageByteByteStringDecoder, "PIMP_STATE_OBSERVER" },
  { PIMP_STATE_PROPERTY, &MessageByteByteBoolByteByteDecoder, "PIMP_STATE_PROPERTY" },
  { PIMP_STATE_CARD, &MessageByteByteDecoder, "PIMP_STATE_CARD" },
  { PIMP_STATE_POT, &MessageUnsignedIntDecoder, "PIMP_STATE_POT" },
  { PIMP_START_OF_TURN, &MessageByteBoolDecoder, "PIMP_START_OF_TURN" },
  { PIMP_THROW_DICE, &MessageDecoder, "PIMP_THROW_DICE" },
  { PIMP_DICE_ROLLED, &MessageByteByteByteDecoder, "PIMP_DICE_ROLLED" },
  { PIMP_THREE_DOUBLES, &MessageByteDecoder, "PIMP_THREE_DOUBLES" },
  { PIMP_DICE_MOVED_PLAYER, &MessageByteByteByteDecoder, "PIMP_DICE_MOVED_PLAYER" },
  { PIMP_CARD_MOVED_PLAYER, &MessageByteByteByteDecoder, "PIMP_CARD_MOVED_PLAYER" },
  { PIMP_SQUARE_MOVED_PLAYER, &MessageByteByteByteDecoder, "PIMP_SQUARE_MOVED_PLAYER" },
  { PIMP_PLAYER_PASSING_BY_SQUARE, &MessageByteByteDecoder, "PIMP_PLAYER_PASSING_BY_SQUARE" },
  { PIMP_PLAYER_LANDING_ON_SQUARE, &MessageByteByteDecoder, "PIMP_PLAYER_LANDING_ON_SQUARE" },
  { PIMP_GOING_TO_JAIL, &MessageByteByteDecoder, "PIMP_GOING_TO_JAIL" },
  { PIMP_WAITING_FOR_TRANSACTION, &MessageByteUnsignedIntDecoder, "PIMP_WAITING_FOR_TRANSACTION" },
  { PIMP_CLAIM_RENT, &MessageByteByteDecoder, "PIMP_CLAIM_RENT" },
  { PIMP_CLAIM_GO, &MessageByteDecoder, "PIMP_CLAIM_GO" },
  { PIMP_RENT_COLLECTION_MORATORIUM, &MessageDecoder, "PIMP_RENT_COLLECTION_MORATORIUM" },
  { PIMP_ROLL_AGAIN, &MessageByteDecoder, "PIMP_ROLL_AGAIN" },
  { PIMP_PROPERTY_SALE, &MessageByteByteUnsignedIntDecoder, "PIMP_PROPERTY_SALE" },
  { PIMP_BUY_PROPERTY, &MessageDecoder, "PIMP_BUY_PROPERTY" },
  { PIMP_AUCTION_PROPERTY, &MessageDecoder, "PIMP_AUCTION_PROPERTY" },
  { PIMP_PROPERTY_AUCTION, &MessageByteDecoder, "PIMP_PROPERTY_AUCTION" },
  { PIMP_BID, &MessageUnsignedIntDecoder, "PIMP_BID" },
  { PIMP_PROPERTY_AUCTION_BID, &MessageByteUnsignedIntDecoder, "PIMP_PROPERTY_AUCTION_BID" },
  { PIMP_NO_BID, &MessageDecoder, "PIMP_NO_BID" },
  { PIMP_PROPERTY_AUCTION_NO_BID, &MessageByteDecoder, "PIMP_PROPERTY_AUCTION_NO_BID" },
  { PIMP_PROPERTY_AUCTION_WON, &MessageByteDecoder, "PIMP_PROPERTY_AUCTION_WON" },
  { PIMP_PROPERTY_AUCTION_VOID, &MessageDecoder, "PIMP_PROPERTY_AUCTION_VOID" },
  { PIMP_MORTGAGE_PROPERTY, &MessageByteDecoder, "PIMP_MORTGAGE_PROPERTY" },
  { PIMP_UNMORTGAGE_PROPERTY, &MessageByteDecoder, "PIMP_UNMORTGAGE_PROPERTY" },
  { PIMP_GOT_CARD, &MessageByteByteByteDecoder, "PIMP_GOT_CARD" },
  { PIMP_CARD_COLLECT_OR_RETRY, &MessageByteDecoder, "PIMP_CARD_COLLECT_OR_RETRY" },
  { PIMP_CARD_SELECT_COLLECT, &MessageDecoder, "PIMP_CARD_SELECT_COLLECT" },
  { PIMP_CARD_SELECT_RETRY, &MessageDecoder, "PIMP_CARD_SELECT_RETRY" },
  { PIMP_TAX_SELECT_OPTION, &MessageByteByteDecoder, "PIMP_TAX_SELECT_OPTION" },
  { PIMP_TAX_PAY_TEN_PERCENT, &MessageDecoder, "PIMP_TAX_PAY_TEN_PERCENT" },
  { PIMP_TAX_PAY_FLAT_FEE, &MessageDecoder, "PIMP_TAX_PAY_FLAT_FEE" },
  { PIMP_JAIL_PAY_OR_ROLL, &MessageByteByteBoolDecoder, "PIMP_JAIL_PAY_OR_ROLL" },
  { PIMP_JAIL_PAY_BAIL, &MessageDecoder, "PIMP_JAIL_PAY_BAIL" },
  { PIMP_JAIL_ROLL_DICE, &MessageDecoder, "PIMP_JAIL_ROLL_DICE" },
  { PIMP_JAIL_FREE, &MessageByteDecoder, "PIMP_JAIL_FREE" },
  { PIMP_TRANSACTION_REQUEST_TRADE, &MessageByteDecoder, "PIMP_TRANSACTION_REQUEST_TRADE" },
  { PIMP_TRANSACTION_TRADE_REQUESTED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_TRADE_REQUESTED" },
  { PIMP_TRANSACTION_RENT_REQUESTED, &MessageUnsignedIntByteByteByteUnsignedIntDecoder, "PIMP_TRANSACTION_RENT_REQUESTED" },
  { PIMP_TRANSACTION_CARD_REQUESTED, &MessageUnsignedIntByteByteByteUnsignedIntDecoder, "PIMP_TRANSACTION_CARD_REQUESTED" },
  { PIMP_TRANSACTION_SQUARE_REQUESTED, &MessageUnsignedIntByteUnsignedIntDecoder, "PIMP_TRANSACTION_SQUARE_REQUESTED" },
  { PIMP_TRANSACTION_BANK_REQUESTED, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_TRANSACTION_BANK_REQUESTED" },
  { PIMP_TRANSACTION_JAIL_REQUESTED, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_TRANSACTION_JAIL_REQUESTED" },
  { PIMP_TRANSACTION_SET_CASH, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_TRANSACTION_SET_CASH" },
  { PIMP_TRANSACTION_CASH_SET, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_TRANSACTION_CASH_SET" },
  { PIMP_TRANSACTION_OTHER_CASH_SET, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_TRANSACTION_OTHER_CASH_SET" },
  { PIMP_TRANSACTION_ADD_PROPERTY, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_ADD_PROPERTY" },
  { PIMP_TRANSACTION_REMOVE_PROPERTY, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_REMOVE_PROPERTY" },
  { PIMP_TRANSACTION_PROPERTY_ADDED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_PROPERTY_ADDED" },
  { PIMP_TRANSACTION_PROPERTY_REMOVED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_PROPERTY_REMOVED" },
  { PIMP_TRANSACTION_OTHER_PROPERTY_ADDED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_OTHER_PROPERTY_ADDED" },
  { PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED" },
  { PIMP_TRANSACTION_ADD_CARD, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_ADD_CARD" },
  { PIMP_TRANSACTION_REMOVE_CARD, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_REMOVE_CARD" },
  { PIMP_TRANSACTION_CARD_ADDED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_CARD_ADDED" },
  { PIMP_TRANSACTION_CARD_REMOVED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_CARD_REMOVED" },
  { PIMP_TRANSACTION_OTHER_CARD_ADDED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_OTHER_CARD_ADDED" },
  { PIMP_TRANSACTION_OTHER_CARD_REMOVED, &MessageUnsignedIntByteDecoder, "PIMP_TRANSACTION_OTHER_CARD_REMOVED" },
  { PIMP_TRANSACTION_FINISH, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_FINISH" },
  { PIMP_TRANSACTION_FINISHED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_FINISHED" },
  { PIMP_TRANSACTION_OTHER_FINISHED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_OTHER_FINISHED" },
  { PIMP_TRANSACTION_REOPEN, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_REOPEN" },
  { PIMP_TRANSACTION_REOPENED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_REOPENED" },
  { PIMP_TRANSACTION_OTHER_REOPENED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_OTHER_REOPENED" },
  { PIMP_TRANSACTION_AGREE, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_AGREE" },
  { PIMP_TRANSACTION_AGREED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_AGREED" },
  { PIMP_TRANSACTION_OTHER_AGREED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_OTHER_AGREED" },
  { PIMP_TRANSACTION_FINALISED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_FINALISED" },
  { PIMP_BANKRUPT_TRANSACTION, &MessageUnsignedIntDecoder, "PIMP_BANKRUPT_TRANSACTION" },
  { PIMP_TRANSACTION_UNUSUAL, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_UNUSUAL" },
  { PIMP_TRANSACTION_CANCEL, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_CANCEL" },
  { PIMP_TRANSACTION_CANCELLED, &MessageUnsignedIntDecoder, "PIMP_TRANSACTION_CANCELLED" },
  { PIMP_SQUARE_GIVES_CASH, &MessageByteByteUnsignedIntDecoder, "PIMP_SQUARE_GIVES_CASH" },
  { PIMP_SQUARE_TAKES_CASH, &MessageByteByteUnsignedIntDecoder, "PIMP_SQUARE_TAKES_CASH" },
  { PIMP_SQUARE_GIVES_CARD, &MessageByteByteByteDecoder, "PIMP_SQUARE_GIVES_CARD" },
  { PIMP_SQUARE_TAKES_CARD, &MessageByteByteByteDecoder, "PIMP_SQUARE_TAKES_CARD" },
  { PIMP_CONSTRUCTION_TAKES_CASH, &MessageByteUnsignedIntDecoder, "PIMP_CONSTRUCTION_TAKES_CASH" },
  { PIMP_DESTRUCTION_GIVES_CASH, &MessageByteUnsignedIntDecoder, "PIMP_DESTRUCTION_GIVES_CASH" },
  { PIMP_PLAYER_CLAIMED_RENT, &MessageByteByteByteUnsignedIntDecoder, "PIMP_PLAYER_CLAIMED_RENT" },
  { PIMP_PLAYER_CLAIMED_GO, &MessageByteByteUnsignedIntDecoder, "PIMP_PLAYER_CLAIMED_GO" },
  { PIMP_PURCHASE_HOUSES, &MessageHouseChangeListDecoder, "PIMP_PURCHASE_HOUSES" },
  { PIMP_ERROR_HOUSES_NOT_OWNED, &MessageByteDecoder, "PIMP_ERROR_HOUSES_NOT_OWNED" },
  { PIMP_ERROR_HOUSES_NEED_MONOPOLY, &MessageByteDecoder, "PIMP_ERROR_HOUSES_NEED_MONOPOLY" },
  { PIMP_ERROR_HOUSES_MORTGAGED, &MessageByteDecoder, "PIMP_ERROR_HOUSES_MORTGAGED" },
  { PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP, &MessageByteDecoder, "PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP" },
  { PIMP_ERROR_HOUSES_NOT_YOUR_TURN, &MessageDecoder, "PIMP_ERROR_HOUSES_NOT_YOUR_TURN" },
  { PIMP_ERROR_HOUSES_TOO_EXPENSIVE, &MessageUnsignedIntDecoder, "PIMP_ERROR_HOUSES_TOO_EXPENSIVE" },
  { PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT, &MessageByteByteDecoder, "PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT" },
  { PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER, &MessageByteDecoder, "PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER" },
  { PIMP_ERROR_HOUSES_UNBALANCED, &MessageByteDecoder, "PIMP_ERROR_HOUSES_UNBALANCED" },
  { PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH, &MessageUnsignedIntDecoder, "PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH" },
  { PIMP_ERROR_HOUSES_TERMITES, &MessageDecoder, "PIMP_ERROR_HOUSES_TERMITES" },
  { PIMP_DELTA_CASH, &MessageByteByteUnsignedIntDecoder, "PIMP_DELTA_CASH" },
  { PIMP_DELTA_PROPERTY, &MessageByteByteByteDecoder, "PIMP_DELTA_PROPERTY" },
  { PIMP_DELTA_CARD, &MessageByteByteByteDecoder, "PIMP_DELTA_CARD" },
  { PIMP_DELTA_HOUSES_PURCHASED, &MessageByteByteByteByteDecoder, "PIMP_DELTA_HOUSES_PURCHASED" },
  { PIMP_DELTA_BANK, &MessageByteByteDecoder, "PIMP_DELTA_BANK" },
  { PIMP_DELTA_PROPERTY_MORTGAGED, &MessageByteDecoder, "PIMP_DELTA_PROPERTY_MORTGAGED" },
  { PIMP_DELTA_PROPERTY_UNMORTGAGED, &MessageByteDecoder, "PIMP_DELTA_PROPERTY_UNMORTGAGED" },
  { PIMP_DELTA_POT, &MessageUnsignedIntDecoder, "PIMP_DELTA_POT" },
  { PIMP_PING, &MessageUnsignedIntDecoder, "PIMP_PING" },
  { PIMP_PONG, &MessageUnsignedIntDecoder, "PIMP_PONG" },
  { PIMP_LINK_DEAD, &MessageByteDecoder, "PIMP_LINK_DEAD" },
  { PIMP_KICK, &MessageByteDecoder, "PIMP_KICK" },
  { PIMP_TRANSFER, &MessageByteDecoder, "PIMP_TRANSFER" },
  { PIMP_AFK, &MessageDecoder, "PIMP_AFK" },
  { PIMP_PLAYER_BECAME_OBSERVER_TRANSFER, &MessageByteDecoder, "PIMP_PLAYER_BECAME_OBSERVER_TRANSFER" },
  { PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT, &MessageByteDecoder, "PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT" },
  { PIMP_PLAYER_BECAME_OBSERVER_KICKED, &MessageByteDecoder, "PIMP_PLAYER_BECAME_OBSERVER_KICKED" },
  { PIMP_PLAYER_WON, &MessageByteDecoder, "PIMP_PLAYER_WON" },
  { PIMP_ERROR_INVALID_RENT_CLAIM, &MessageDecoder, "PIMP_ERROR_INVALID_RENT_CLAIM" },
  { PIMP_ERROR_INVALID_GO_CLAIM, &MessageDecoder, "PIMP_ERROR_INVALID_GO_CLAIM" },
  { PIMP_ERROR_PROPERTY_TOO_EXPENSIVE, &MessageByteUnsignedIntDecoder, "PIMP_ERROR_PROPERTY_TOO_EXPENSIVE" },
  { PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE, &MessageByteUnsignedIntDecoder, "PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE" },
  { PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE" },
  { PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED, &MessageUnsignedIntByteDecoder, "PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED" },
  { PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED, &MessageUnsignedIntByteDecoder, "PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED" },
  { PIMP_ERROR_TRANSACTION_NOT_SUITABLE, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_ERROR_TRANSACTION_NOT_SUITABLE" },
  { PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE, &MessageUnsignedIntByteDecoder, "PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE" },
  { PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH" },
  { PIMP_ERROR_TRANSACTION_NOT_BANKRUPT, &MessageUnsignedIntUnsignedIntDecoder, "PIMP_ERROR_TRANSACTION_NOT_BANKRUPT" },
  { PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED, &MessageUnsignedIntDecoder, "PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED" },
  { PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION, &MessageUnsignedIntDecoder, "PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION" },
  { PIMP_MONEY_BEING_HELD_IN_ESCROW, &MessageByteUnsignedIntUnsignedIntDecoder, "PIMP_MONEY_BEING_HELD_IN_ESCROW" },
  { PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH, &MessageByteUnsignedIntDecoder, "PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH" },
  { PIMP_ERROR_UNKNOWN_PROTOCOL, &MessageDecoder, "PIMP_ERROR_UNKNOWN_PROTOCOL" },
  { PIMP_ERROR_TOO_MANY_USERS, &MessageDecoder, "PIMP_ERROR_TOO_MANY_USERS" },
  { PIMP_ERROR_NAME_IN_USE, &MessageDecoder, "PIMP_ERROR_NAME_IN_USE" },
  { PIMP_ERROR_NOT_WELCOME, &MessageDecoder, "PIMP_ERROR_NOT_WELCOME" },
  { PIMP_ERROR_WRONG_PASSWORD, &MessageDecoder, "PIMP_ERROR_WRONG_PASSWORD" },
  { PIMP_ERROR_INVALID_PAYLOAD, &MessageByteDecoder, "PIMP_ERROR_INVALID_PAYLOAD" },
  { PIMP_ERROR_UNPARSEABLE, &MessageDecoder, "PIMP_ERROR_UNPARSEABLE" },
  { PIMP_ERROR_UNEXPECTED_MESSAGE, &MessageByteDecoder, "PIMP_ERROR_UNEXPECTED_MESSAGE" }
};
