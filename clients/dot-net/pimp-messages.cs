using System;  // -*- Mode: Java -*-

          class PIMP_ERROR_UNEXPECTED_MESSAGE : Message {
              public const byte MessageType = 0xFE;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_UNEXPECTED_MESSAGE() : base() { }
              public PIMP_ERROR_UNEXPECTED_MESSAGE(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_UNPARSEABLE : Message {
              public const byte MessageType = 0xFD;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_UNPARSEABLE() : base() { }

          }

          class PIMP_ERROR_INVALID_PAYLOAD : Message {
              public const byte MessageType = 0xFC;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_INVALID_PAYLOAD() : base() { }
              public PIMP_ERROR_INVALID_PAYLOAD(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_HANDSHAKE : Message {
              public const byte MessageType = 0x00;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_HANDSHAKE() : base() { }
              public PIMP_HANDSHAKE(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_HANDSHAKE_ACKNOWLEDGE : Message {
              public const byte MessageType = 0x01;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_HANDSHAKE_ACKNOWLEDGE() : base() { }
              public PIMP_HANDSHAKE_ACKNOWLEDGE(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_UNKNOWN_PROTOCOL : Message {
              public const byte MessageType = 0xF0;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_UNKNOWN_PROTOCOL() : base() { }

          }

          class PIMP_JOIN : Message {
              public const byte MessageType = 0x02;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public bool Field2;
              public string Field3;
              public PIMP_JOIN() : base() { }
              public PIMP_JOIN(byte field1, bool field2, string field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_QUERY_JOIN_PLAY : Message {
              public const byte MessageType = 0x06;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public string Field2;
              public PIMP_QUERY_JOIN_PLAY() : base() { }
              public PIMP_QUERY_JOIN_PLAY(uint field1, string field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_QUERY_JOIN_OBSERVE : Message {
              public const byte MessageType = 0x07;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public string Field2;
              public PIMP_QUERY_JOIN_OBSERVE() : base() { }
              public PIMP_QUERY_JOIN_OBSERVE(uint field1, string field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_JOIN_PENDING : Message {
              public const byte MessageType = 0x10;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_JOIN_PENDING() : base() { }

          }

          class PIMP_ACCEPT_JOIN : Message {
              public const byte MessageType = 0x08;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_ACCEPT_JOIN() : base() { }
              public PIMP_ACCEPT_JOIN(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_REFUSE_JOIN : Message {
              public const byte MessageType = 0x09;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_REFUSE_JOIN() : base() { }
              public PIMP_REFUSE_JOIN(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_JOIN_REFUSED : Message {
              public const byte MessageType = 0x0F;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public string Field2;
              public PIMP_JOIN_REFUSED() : base() { }
              public PIMP_JOIN_REFUSED(uint field1, string field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TOO_MANY_USERS : Message {
              public const byte MessageType = 0xF1;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_TOO_MANY_USERS() : base() { }

          }

          class PIMP_ERROR_NAME_IN_USE : Message {
              public const byte MessageType = 0xF2;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_NAME_IN_USE() : base() { }

          }

          class PIMP_ERROR_NOT_WELCOME : Message {
              public const byte MessageType = 0xF3;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_NOT_WELCOME() : base() { }

          }

          class PIMP_WELCOME_DETAILS : Message {
              public const byte MessageType = 0x03;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_WELCOME_DETAILS() : base() { }
              public PIMP_WELCOME_DETAILS(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_WELCOME_PLAYER : Message {
              public const byte MessageType = 0x04;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public string Field3;
              public PIMP_WELCOME_PLAYER() : base() { }
              public PIMP_WELCOME_PLAYER(byte field1, byte field2, string field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_WELCOME_OBSERVER : Message {
              public const byte MessageType = 0x05;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public string Field3;
              public PIMP_WELCOME_OBSERVER() : base() { }
              public PIMP_WELCOME_OBSERVER(byte field1, byte field2, string field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_REJOIN : Message {
              public const byte MessageType = 0x0A;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_REJOIN() : base() { }
              public PIMP_REJOIN(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_WELCOME_BACK : Message {
              public const byte MessageType = 0x0B;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public string Field3;
              public bool Field4;
              public PIMP_WELCOME_BACK() : base() { }
              public PIMP_WELCOME_BACK(byte field1, byte field2, string field3, bool field4) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
              }
          }

          class PIMP_ERROR_WRONG_PASSWORD : Message {
              public const byte MessageType = 0xF4;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_WRONG_PASSWORD() : base() { }

          }

          class PIMP_PING : Message {
              public const byte MessageType = 0xD0;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_PING() : base() { }
              public PIMP_PING(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PONG : Message {
              public const byte MessageType = 0xD1;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_PONG() : base() { }
              public PIMP_PONG(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_LINK_DEAD : Message {
              public const byte MessageType = 0xD2;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_LINK_DEAD() : base() { }
              public PIMP_LINK_DEAD(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_KICK : Message {
              public const byte MessageType = 0xD3;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_KICK() : base() { }
              public PIMP_KICK(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_BECAME_OBSERVER_KICKED : Message {
              public const byte MessageType = 0xDC;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PLAYER_BECAME_OBSERVER_KICKED() : base() { }
              public PIMP_PLAYER_BECAME_OBSERVER_KICKED(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSFER : Message {
              public const byte MessageType = 0xD4;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_TRANSFER() : base() { }
              public PIMP_TRANSFER(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION : Message {
              public const byte MessageType = 0xEC;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION() : base() { }
              public PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_TAKEOVER : Message {
              public const byte MessageType = 0x0C;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_PLAYER_TAKEOVER() : base() { }
              public PIMP_PLAYER_TAKEOVER(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_AFK : Message {
              public const byte MessageType = 0xD5;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_AFK() : base() { }

          }

          class PIMP_SWITCH_PLAY : Message {
              public const byte MessageType = 0x0D;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_SWITCH_PLAY() : base() { }

          }

          class PIMP_OBSERVER_BECAME_PLAYER : Message {
              public const byte MessageType = 0x0E;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_OBSERVER_BECAME_PLAYER() : base() { }
              public PIMP_OBSERVER_BECAME_PLAYER(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_BECAME_OBSERVER_TRANSFER : Message {
              public const byte MessageType = 0xDA;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PLAYER_BECAME_OBSERVER_TRANSFER() : base() { }
              public PIMP_PLAYER_BECAME_OBSERVER_TRANSFER(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT : Message {
              public const byte MessageType = 0xDB;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT() : base() { }
              public PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_WON : Message {
              public const byte MessageType = 0xDF;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PLAYER_WON() : base() { }
              public PIMP_PLAYER_WON(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_REQUEST_STATE : Message {
              public const byte MessageType = 0x11;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_REQUEST_STATE() : base() { }

          }

          class PIMP_STATE_BOARD : Message {
              public const byte MessageType = 0x12;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_STATE_BOARD() : base() { }
              public PIMP_STATE_BOARD(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_STATE_PLAYER : Message {
              public const byte MessageType = 0x13;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public string Field3;
              public byte Field4;
              public uint Field5;
              public byte Field6;
              public PIMP_STATE_PLAYER() : base() { }
              public PIMP_STATE_PLAYER(byte field1, byte field2, string field3, byte field4, uint field5, byte field6) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
                  Field5 = field5;
                  Field6 = field6;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  Write(Field5, ref payload, ref index);
                  Write(Field6, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
                  Read(out Field5, ref payload, ref index);
                  Read(out Field6, ref payload, ref index);
              }
          }

          class PIMP_STATE_OBSERVER : Message {
              public const byte MessageType = 0x14;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public string Field3;
              public PIMP_STATE_OBSERVER() : base() { }
              public PIMP_STATE_OBSERVER(byte field1, byte field2, string field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_STATE_PROPERTY : Message {
              public const byte MessageType = 0x15;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public bool Field3;
              public byte Field4;
              public byte Field5;
              public PIMP_STATE_PROPERTY() : base() { }
              public PIMP_STATE_PROPERTY(byte field1, byte field2, bool field3, byte field4, byte field5) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
                  Field5 = field5;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  Write(Field5, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
                  Read(out Field5, ref payload, ref index);
              }
          }

          class PIMP_STATE_CARD : Message {
              public const byte MessageType = 0x16;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_STATE_CARD() : base() { }
              public PIMP_STATE_CARD(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_STATE_POT : Message {
              public const byte MessageType = 0x1F;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_STATE_POT() : base() { }
              public PIMP_STATE_POT(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_START_OF_TURN : Message {
              public const byte MessageType = 0x20;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public bool Field2;
              public PIMP_START_OF_TURN() : base() { }
              public PIMP_START_OF_TURN(byte field1, bool field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_THROW_DICE : Message {
              public const byte MessageType = 0x21;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_THROW_DICE() : base() { }

          }

          class PIMP_DICE_ROLLED : Message {
              public const byte MessageType = 0x22;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_DICE_ROLLED() : base() { }
              public PIMP_DICE_ROLLED(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_THREE_DOUBLES : Message {
              public const byte MessageType = 0x23;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_THREE_DOUBLES() : base() { }
              public PIMP_THREE_DOUBLES(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_DICE_MOVED_PLAYER : Message {
              public const byte MessageType = 0x24;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_DICE_MOVED_PLAYER() : base() { }
              public PIMP_DICE_MOVED_PLAYER(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_CARD_MOVED_PLAYER : Message {
              public const byte MessageType = 0x25;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_CARD_MOVED_PLAYER() : base() { }
              public PIMP_CARD_MOVED_PLAYER(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_SQUARE_MOVED_PLAYER : Message {
              public const byte MessageType = 0x26;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_SQUARE_MOVED_PLAYER() : base() { }
              public PIMP_SQUARE_MOVED_PLAYER(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_PASSING_BY_SQUARE : Message {
              public const byte MessageType = 0x27;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_PLAYER_PASSING_BY_SQUARE() : base() { }
              public PIMP_PLAYER_PASSING_BY_SQUARE(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_LANDING_ON_SQUARE : Message {
              public const byte MessageType = 0x28;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_PLAYER_LANDING_ON_SQUARE() : base() { }
              public PIMP_PLAYER_LANDING_ON_SQUARE(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_GOING_TO_JAIL : Message {
              public const byte MessageType = 0x29;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_GOING_TO_JAIL() : base() { }
              public PIMP_GOING_TO_JAIL(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_WAITING_FOR_TRANSACTION : Message {
              public const byte MessageType = 0x2A;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_WAITING_FOR_TRANSACTION() : base() { }
              public PIMP_WAITING_FOR_TRANSACTION(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_CLAIM_RENT : Message {
              public const byte MessageType = 0x2B;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_CLAIM_RENT() : base() { }
              public PIMP_CLAIM_RENT(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_INVALID_RENT_CLAIM : Message {
              public const byte MessageType = 0xE0;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_INVALID_RENT_CLAIM() : base() { }

          }

          class PIMP_CLAIM_GO : Message {
              public const byte MessageType = 0x2C;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_CLAIM_GO() : base() { }
              public PIMP_CLAIM_GO(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_INVALID_GO_CLAIM : Message {
              public const byte MessageType = 0xE1;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_INVALID_GO_CLAIM() : base() { }

          }

          class PIMP_MONEY_BEING_HELD_IN_ESCROW : Message {
              public const byte MessageType = 0xED;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public uint Field3;
              public PIMP_MONEY_BEING_HELD_IN_ESCROW() : base() { }
              public PIMP_MONEY_BEING_HELD_IN_ESCROW(byte field1, uint field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_RENT_COLLECTION_MORATORIUM : Message {
              public const byte MessageType = 0x2E;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_RENT_COLLECTION_MORATORIUM() : base() { }

          }

          class PIMP_ROLL_AGAIN : Message {
              public const byte MessageType = 0x2F;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ROLL_AGAIN() : base() { }
              public PIMP_ROLL_AGAIN(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PROPERTY_SALE : Message {
              public const byte MessageType = 0x30;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public uint Field3;
              public PIMP_PROPERTY_SALE() : base() { }
              public PIMP_PROPERTY_SALE(byte field1, byte field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_BUY_PROPERTY : Message {
              public const byte MessageType = 0x31;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_BUY_PROPERTY() : base() { }

          }

          class PIMP_ERROR_PROPERTY_TOO_EXPENSIVE : Message {
              public const byte MessageType = 0xE2;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_ERROR_PROPERTY_TOO_EXPENSIVE() : base() { }
              public PIMP_ERROR_PROPERTY_TOO_EXPENSIVE(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH : Message {
              public const byte MessageType = 0xEE;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH() : base() { }
              public PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_AUCTION_PROPERTY : Message {
              public const byte MessageType = 0x32;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_AUCTION_PROPERTY() : base() { }

          }

          class PIMP_PROPERTY_AUCTION : Message {
              public const byte MessageType = 0x33;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PROPERTY_AUCTION() : base() { }
              public PIMP_PROPERTY_AUCTION(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_BID : Message {
              public const byte MessageType = 0x34;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_BID() : base() { }
              public PIMP_BID(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PROPERTY_AUCTION_BID : Message {
              public const byte MessageType = 0x35;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_PROPERTY_AUCTION_BID() : base() { }
              public PIMP_PROPERTY_AUCTION_BID(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_NO_BID : Message {
              public const byte MessageType = 0x36;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_NO_BID() : base() { }

          }

          class PIMP_PROPERTY_AUCTION_NO_BID : Message {
              public const byte MessageType = 0x37;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PROPERTY_AUCTION_NO_BID() : base() { }
              public PIMP_PROPERTY_AUCTION_NO_BID(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PROPERTY_AUCTION_WON : Message {
              public const byte MessageType = 0x38;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_PROPERTY_AUCTION_WON() : base() { }
              public PIMP_PROPERTY_AUCTION_WON(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_PROPERTY_AUCTION_VOID : Message {
              public const byte MessageType = 0x39;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_PROPERTY_AUCTION_VOID() : base() { }

          }

          class PIMP_ERROR_HOUSES_NOT_OWNED : Message {
              public const byte MessageType = 0x91;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_HOUSES_NOT_OWNED() : base() { }
              public PIMP_ERROR_HOUSES_NOT_OWNED(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_NEED_MONOPOLY : Message {
              public const byte MessageType = 0x92;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_HOUSES_NEED_MONOPOLY() : base() { }
              public PIMP_ERROR_HOUSES_NEED_MONOPOLY(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_MORTGAGED : Message {
              public const byte MessageType = 0x93;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_HOUSES_MORTGAGED() : base() { }
              public PIMP_ERROR_HOUSES_MORTGAGED(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP : Message {
              public const byte MessageType = 0x94;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP() : base() { }
              public PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_NOT_YOUR_TURN : Message {
              public const byte MessageType = 0x95;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_HOUSES_NOT_YOUR_TURN() : base() { }

          }

          class PIMP_ERROR_HOUSES_TOO_EXPENSIVE : Message {
              public const byte MessageType = 0x96;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_ERROR_HOUSES_TOO_EXPENSIVE() : base() { }
              public PIMP_ERROR_HOUSES_TOO_EXPENSIVE(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT : Message {
              public const byte MessageType = 0x97;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT() : base() { }
              public PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER : Message {
              public const byte MessageType = 0x98;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER() : base() { }
              public PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_UNBALANCED : Message {
              public const byte MessageType = 0x99;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_ERROR_HOUSES_UNBALANCED() : base() { }
              public PIMP_ERROR_HOUSES_UNBALANCED(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH : Message {
              public const byte MessageType = 0x9A;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH() : base() { }
              public PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_HOUSES_TERMITES : Message {
              public const byte MessageType = 0x9B;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_ERROR_HOUSES_TERMITES() : base() { }

          }

          class PIMP_DELTA_HOUSES_PURCHASED : Message {
              public const byte MessageType = 0xC3;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public byte Field4;
              public PIMP_DELTA_HOUSES_PURCHASED() : base() { }
              public PIMP_DELTA_HOUSES_PURCHASED(byte field1, byte field2, byte field3, byte field4) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
              }
          }

          class PIMP_MORTGAGE_PROPERTY : Message {
              public const byte MessageType = 0x3B;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_MORTGAGE_PROPERTY() : base() { }
              public PIMP_MORTGAGE_PROPERTY(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_DELTA_PROPERTY_MORTGAGED : Message {
              public const byte MessageType = 0xC5;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_DELTA_PROPERTY_MORTGAGED() : base() { }
              public PIMP_DELTA_PROPERTY_MORTGAGED(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_UNMORTGAGE_PROPERTY : Message {
              public const byte MessageType = 0x3C;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_UNMORTGAGE_PROPERTY() : base() { }
              public PIMP_UNMORTGAGE_PROPERTY(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_DELTA_PROPERTY_UNMORTGAGED : Message {
              public const byte MessageType = 0xC6;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_DELTA_PROPERTY_UNMORTGAGED() : base() { }
              public PIMP_DELTA_PROPERTY_UNMORTGAGED(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE : Message {
              public const byte MessageType = 0xE3;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE() : base() { }
              public PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_GOT_CARD : Message {
              public const byte MessageType = 0x40;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_GOT_CARD() : base() { }
              public PIMP_GOT_CARD(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_CARD_COLLECT_OR_RETRY : Message {
              public const byte MessageType = 0x41;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_CARD_COLLECT_OR_RETRY() : base() { }
              public PIMP_CARD_COLLECT_OR_RETRY(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_CARD_SELECT_COLLECT : Message {
              public const byte MessageType = 0x42;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_CARD_SELECT_COLLECT() : base() { }

          }

          class PIMP_CARD_SELECT_RETRY : Message {
              public const byte MessageType = 0x43;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_CARD_SELECT_RETRY() : base() { }

          }

          class PIMP_TAX_SELECT_OPTION : Message {
              public const byte MessageType = 0x45;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_TAX_SELECT_OPTION() : base() { }
              public PIMP_TAX_SELECT_OPTION(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TAX_PAY_TEN_PERCENT : Message {
              public const byte MessageType = 0x46;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_TAX_PAY_TEN_PERCENT() : base() { }

          }

          class PIMP_TAX_PAY_FLAT_FEE : Message {
              public const byte MessageType = 0x47;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_TAX_PAY_FLAT_FEE() : base() { }

          }

          class PIMP_JAIL_PAY_OR_ROLL : Message {
              public const byte MessageType = 0x4A;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public bool Field3;
              public PIMP_JAIL_PAY_OR_ROLL() : base() { }
              public PIMP_JAIL_PAY_OR_ROLL(byte field1, byte field2, bool field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_JAIL_PAY_BAIL : Message {
              public const byte MessageType = 0x4B;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_JAIL_PAY_BAIL() : base() { }

          }

          class PIMP_JAIL_ROLL_DICE : Message {
              public const byte MessageType = 0x4C;
              override public byte GetMessageType() { return MessageType; }
              public PIMP_JAIL_ROLL_DICE() : base() { }

          }

          class PIMP_JAIL_FREE : Message {
              public const byte MessageType = 0x4D;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_JAIL_FREE() : base() { }
              public PIMP_JAIL_FREE(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_REQUEST_TRADE : Message {
              public const byte MessageType = 0x50;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public PIMP_TRANSACTION_REQUEST_TRADE() : base() { }
              public PIMP_TRANSACTION_REQUEST_TRADE(byte field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_TRADE_REQUESTED : Message {
              public const byte MessageType = 0x51;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_TRADE_REQUESTED() : base() { }
              public PIMP_TRANSACTION_TRADE_REQUESTED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_RENT_REQUESTED : Message {
              public const byte MessageType = 0x52;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public byte Field3;
              public byte Field4;
              public uint Field5;
              public PIMP_TRANSACTION_RENT_REQUESTED() : base() { }
              public PIMP_TRANSACTION_RENT_REQUESTED(uint field1, byte field2, byte field3, byte field4, uint field5) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
                  Field5 = field5;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  Write(Field5, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
                  Read(out Field5, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_CARD_REQUESTED : Message {
              public const byte MessageType = 0x53;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public byte Field3;
              public byte Field4;
              public uint Field5;
              public PIMP_TRANSACTION_CARD_REQUESTED() : base() { }
              public PIMP_TRANSACTION_CARD_REQUESTED(uint field1, byte field2, byte field3, byte field4, uint field5) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
                  Field5 = field5;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  Write(Field5, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
                  Read(out Field5, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_SQUARE_REQUESTED : Message {
              public const byte MessageType = 0x54;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public uint Field3;
              public PIMP_TRANSACTION_SQUARE_REQUESTED() : base() { }
              public PIMP_TRANSACTION_SQUARE_REQUESTED(uint field1, byte field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_BANK_REQUESTED : Message {
              public const byte MessageType = 0x55;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_TRANSACTION_BANK_REQUESTED() : base() { }
              public PIMP_TRANSACTION_BANK_REQUESTED(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_JAIL_REQUESTED : Message {
              public const byte MessageType = 0x56;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_TRANSACTION_JAIL_REQUESTED() : base() { }
              public PIMP_TRANSACTION_JAIL_REQUESTED(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_SET_CASH : Message {
              public const byte MessageType = 0x60;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_TRANSACTION_SET_CASH() : base() { }
              public PIMP_TRANSACTION_SET_CASH(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE : Message {
              public const byte MessageType = 0xE4;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE() : base() { }
              public PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_CASH_SET : Message {
              public const byte MessageType = 0x61;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_TRANSACTION_CASH_SET() : base() { }
              public PIMP_TRANSACTION_CASH_SET(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_CASH_SET : Message {
              public const byte MessageType = 0x62;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_TRANSACTION_OTHER_CASH_SET() : base() { }
              public PIMP_TRANSACTION_OTHER_CASH_SET(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_ADD_PROPERTY : Message {
              public const byte MessageType = 0x63;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_ADD_PROPERTY() : base() { }
              public PIMP_TRANSACTION_ADD_PROPERTY(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED : Message {
              public const byte MessageType = 0xE5;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED() : base() { }
              public PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_REMOVE_PROPERTY : Message {
              public const byte MessageType = 0x64;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_REMOVE_PROPERTY() : base() { }
              public PIMP_TRANSACTION_REMOVE_PROPERTY(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_PROPERTY_ADDED : Message {
              public const byte MessageType = 0x65;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_PROPERTY_ADDED() : base() { }
              public PIMP_TRANSACTION_PROPERTY_ADDED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_PROPERTY_REMOVED : Message {
              public const byte MessageType = 0x66;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_PROPERTY_REMOVED() : base() { }
              public PIMP_TRANSACTION_PROPERTY_REMOVED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_PROPERTY_ADDED : Message {
              public const byte MessageType = 0x67;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_OTHER_PROPERTY_ADDED() : base() { }
              public PIMP_TRANSACTION_OTHER_PROPERTY_ADDED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED : Message {
              public const byte MessageType = 0x68;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED() : base() { }
              public PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_ADD_CARD : Message {
              public const byte MessageType = 0x69;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_ADD_CARD() : base() { }
              public PIMP_TRANSACTION_ADD_CARD(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED : Message {
              public const byte MessageType = 0xE6;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED() : base() { }
              public PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_REMOVE_CARD : Message {
              public const byte MessageType = 0x6A;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_REMOVE_CARD() : base() { }
              public PIMP_TRANSACTION_REMOVE_CARD(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_CARD_ADDED : Message {
              public const byte MessageType = 0x6B;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_CARD_ADDED() : base() { }
              public PIMP_TRANSACTION_CARD_ADDED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_CARD_REMOVED : Message {
              public const byte MessageType = 0x6C;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_CARD_REMOVED() : base() { }
              public PIMP_TRANSACTION_CARD_REMOVED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_CARD_ADDED : Message {
              public const byte MessageType = 0x6D;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_OTHER_CARD_ADDED() : base() { }
              public PIMP_TRANSACTION_OTHER_CARD_ADDED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_CARD_REMOVED : Message {
              public const byte MessageType = 0x6E;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_TRANSACTION_OTHER_CARD_REMOVED() : base() { }
              public PIMP_TRANSACTION_OTHER_CARD_REMOVED(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_FINISH : Message {
              public const byte MessageType = 0x70;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_FINISH() : base() { }
              public PIMP_TRANSACTION_FINISH(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_FINISHED : Message {
              public const byte MessageType = 0x71;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_FINISHED() : base() { }
              public PIMP_TRANSACTION_FINISHED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_FINISHED : Message {
              public const byte MessageType = 0x72;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_OTHER_FINISHED() : base() { }
              public PIMP_TRANSACTION_OTHER_FINISHED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_NOT_SUITABLE : Message {
              public const byte MessageType = 0xE7;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_ERROR_TRANSACTION_NOT_SUITABLE() : base() { }
              public PIMP_ERROR_TRANSACTION_NOT_SUITABLE(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_REOPEN : Message {
              public const byte MessageType = 0x73;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_REOPEN() : base() { }
              public PIMP_TRANSACTION_REOPEN(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_REOPENED : Message {
              public const byte MessageType = 0x74;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_REOPENED() : base() { }
              public PIMP_TRANSACTION_REOPENED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_REOPENED : Message {
              public const byte MessageType = 0x75;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_OTHER_REOPENED() : base() { }
              public PIMP_TRANSACTION_OTHER_REOPENED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_AGREE : Message {
              public const byte MessageType = 0x76;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_AGREE() : base() { }
              public PIMP_TRANSACTION_AGREE(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE : Message {
              public const byte MessageType = 0xE8;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public byte Field2;
              public PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE() : base() { }
              public PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE(uint field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH : Message {
              public const byte MessageType = 0xE9;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH() : base() { }
              public PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_AGREED : Message {
              public const byte MessageType = 0x77;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_AGREED() : base() { }
              public PIMP_TRANSACTION_AGREED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_OTHER_AGREED : Message {
              public const byte MessageType = 0x78;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_OTHER_AGREED() : base() { }
              public PIMP_TRANSACTION_OTHER_AGREED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_FINALISED : Message {
              public const byte MessageType = 0x79;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_FINALISED() : base() { }
              public PIMP_TRANSACTION_FINALISED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_BANKRUPT_TRANSACTION : Message {
              public const byte MessageType = 0x7A;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_BANKRUPT_TRANSACTION() : base() { }
              public PIMP_BANKRUPT_TRANSACTION(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_NOT_BANKRUPT : Message {
              public const byte MessageType = 0xEA;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public uint Field2;
              public PIMP_ERROR_TRANSACTION_NOT_BANKRUPT() : base() { }
              public PIMP_ERROR_TRANSACTION_NOT_BANKRUPT(uint field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_UNUSUAL : Message {
              public const byte MessageType = 0x7D;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_UNUSUAL() : base() { }
              public PIMP_TRANSACTION_UNUSUAL(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_CANCEL : Message {
              public const byte MessageType = 0x7E;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_CANCEL() : base() { }
              public PIMP_TRANSACTION_CANCEL(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_TRANSACTION_CANCELLED : Message {
              public const byte MessageType = 0x7F;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_TRANSACTION_CANCELLED() : base() { }
              public PIMP_TRANSACTION_CANCELLED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED : Message {
              public const byte MessageType = 0xEB;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED() : base() { }
              public PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

          class PIMP_SQUARE_GIVES_CASH : Message {
              public const byte MessageType = 0x80;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public uint Field3;
              public PIMP_SQUARE_GIVES_CASH() : base() { }
              public PIMP_SQUARE_GIVES_CASH(byte field1, byte field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_SQUARE_TAKES_CASH : Message {
              public const byte MessageType = 0x81;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public uint Field3;
              public PIMP_SQUARE_TAKES_CASH() : base() { }
              public PIMP_SQUARE_TAKES_CASH(byte field1, byte field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_SQUARE_GIVES_CARD : Message {
              public const byte MessageType = 0x82;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_SQUARE_GIVES_CARD() : base() { }
              public PIMP_SQUARE_GIVES_CARD(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_SQUARE_TAKES_CARD : Message {
              public const byte MessageType = 0x83;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_SQUARE_TAKES_CARD() : base() { }
              public PIMP_SQUARE_TAKES_CARD(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_CONSTRUCTION_TAKES_CASH : Message {
              public const byte MessageType = 0x84;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_CONSTRUCTION_TAKES_CASH() : base() { }
              public PIMP_CONSTRUCTION_TAKES_CASH(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_DESTRUCTION_GIVES_CASH : Message {
              public const byte MessageType = 0x85;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public uint Field2;
              public PIMP_DESTRUCTION_GIVES_CASH() : base() { }
              public PIMP_DESTRUCTION_GIVES_CASH(byte field1, uint field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_CLAIMED_RENT : Message {
              public const byte MessageType = 0x86;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public uint Field4;
              public PIMP_PLAYER_CLAIMED_RENT() : base() { }
              public PIMP_PLAYER_CLAIMED_RENT(byte field1, byte field2, byte field3, uint field4) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
                  Field4 = field4;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  Write(Field4, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
                  Read(out Field4, ref payload, ref index);
              }
          }

          class PIMP_PLAYER_CLAIMED_GO : Message {
              public const byte MessageType = 0x87;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public uint Field3;
              public PIMP_PLAYER_CLAIMED_GO() : base() { }
              public PIMP_PLAYER_CLAIMED_GO(byte field1, byte field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_DELTA_CASH : Message {
              public const byte MessageType = 0xC0;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public uint Field3;
              public PIMP_DELTA_CASH() : base() { }
              public PIMP_DELTA_CASH(byte field1, byte field2, uint field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_DELTA_PROPERTY : Message {
              public const byte MessageType = 0xC1;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_DELTA_PROPERTY() : base() { }
              public PIMP_DELTA_PROPERTY(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_DELTA_CARD : Message {
              public const byte MessageType = 0xC2;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public byte Field3;
              public PIMP_DELTA_CARD() : base() { }
              public PIMP_DELTA_CARD(byte field1, byte field2, byte field3) : base() {
                  Field1 = field1;
                  Field2 = field2;
                  Field3 = field3;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  Write(Field3, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
                  Read(out Field3, ref payload, ref index);
              }
          }

          class PIMP_DELTA_BANK : Message {
              public const byte MessageType = 0xC4;
              override public byte GetMessageType() { return MessageType; }
              public byte Field1;
              public byte Field2;
              public PIMP_DELTA_BANK() : base() { }
              public PIMP_DELTA_BANK(byte field1, byte field2) : base() {
                  Field1 = field1;
                  Field2 = field2;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  Write(Field2, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
                  Read(out Field2, ref payload, ref index);
              }
          }

          class PIMP_DELTA_POT : Message {
              public const byte MessageType = 0xCF;
              override public byte GetMessageType() { return MessageType; }
              public uint Field1;
              public PIMP_DELTA_POT() : base() { }
              public PIMP_DELTA_POT(uint field1) : base() {
                  Field1 = field1;
              }
              override protected void SerializePayload(ref byte[] payload, ref byte index) {
                  Write(Field1, ref payload, ref index);
                  }
              override protected void ParsePayload(ref byte[] payload) {
                  byte index = 0;
                  Read(out Field1, ref payload, ref index);
              }
          }

class MessageMap {
    public static Message CreateMessage(byte type) {
        switch (type) {

            case PIMP_ERROR_UNEXPECTED_MESSAGE.MessageType: return new PIMP_ERROR_UNEXPECTED_MESSAGE();
            case PIMP_ERROR_UNPARSEABLE.MessageType: return new PIMP_ERROR_UNPARSEABLE();
            case PIMP_ERROR_INVALID_PAYLOAD.MessageType: return new PIMP_ERROR_INVALID_PAYLOAD();
            case PIMP_HANDSHAKE.MessageType: return new PIMP_HANDSHAKE();
            case PIMP_HANDSHAKE_ACKNOWLEDGE.MessageType: return new PIMP_HANDSHAKE_ACKNOWLEDGE();
            case PIMP_ERROR_UNKNOWN_PROTOCOL.MessageType: return new PIMP_ERROR_UNKNOWN_PROTOCOL();
            case PIMP_JOIN.MessageType: return new PIMP_JOIN();
            case PIMP_QUERY_JOIN_PLAY.MessageType: return new PIMP_QUERY_JOIN_PLAY();
            case PIMP_QUERY_JOIN_OBSERVE.MessageType: return new PIMP_QUERY_JOIN_OBSERVE();
            case PIMP_JOIN_PENDING.MessageType: return new PIMP_JOIN_PENDING();
            case PIMP_ACCEPT_JOIN.MessageType: return new PIMP_ACCEPT_JOIN();
            case PIMP_REFUSE_JOIN.MessageType: return new PIMP_REFUSE_JOIN();
            case PIMP_JOIN_REFUSED.MessageType: return new PIMP_JOIN_REFUSED();
            case PIMP_ERROR_TOO_MANY_USERS.MessageType: return new PIMP_ERROR_TOO_MANY_USERS();
            case PIMP_ERROR_NAME_IN_USE.MessageType: return new PIMP_ERROR_NAME_IN_USE();
            case PIMP_ERROR_NOT_WELCOME.MessageType: return new PIMP_ERROR_NOT_WELCOME();
            case PIMP_WELCOME_DETAILS.MessageType: return new PIMP_WELCOME_DETAILS();
            case PIMP_WELCOME_PLAYER.MessageType: return new PIMP_WELCOME_PLAYER();
            case PIMP_WELCOME_OBSERVER.MessageType: return new PIMP_WELCOME_OBSERVER();
            case PIMP_REJOIN.MessageType: return new PIMP_REJOIN();
            case PIMP_WELCOME_BACK.MessageType: return new PIMP_WELCOME_BACK();
            case PIMP_ERROR_WRONG_PASSWORD.MessageType: return new PIMP_ERROR_WRONG_PASSWORD();
            case PIMP_PING.MessageType: return new PIMP_PING();
            case PIMP_PONG.MessageType: return new PIMP_PONG();
            case PIMP_LINK_DEAD.MessageType: return new PIMP_LINK_DEAD();
            case PIMP_KICK.MessageType: return new PIMP_KICK();
            case PIMP_PLAYER_BECAME_OBSERVER_KICKED.MessageType: return new PIMP_PLAYER_BECAME_OBSERVER_KICKED();
            case PIMP_TRANSFER.MessageType: return new PIMP_TRANSFER();
            case PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION.MessageType: return new PIMP_ERROR_CANNOT_TRANSFER_DURING_TRANSACTION();
            case PIMP_PLAYER_TAKEOVER.MessageType: return new PIMP_PLAYER_TAKEOVER();
            case PIMP_AFK.MessageType: return new PIMP_AFK();
            case PIMP_SWITCH_PLAY.MessageType: return new PIMP_SWITCH_PLAY();
            case PIMP_OBSERVER_BECAME_PLAYER.MessageType: return new PIMP_OBSERVER_BECAME_PLAYER();
            case PIMP_PLAYER_BECAME_OBSERVER_TRANSFER.MessageType: return new PIMP_PLAYER_BECAME_OBSERVER_TRANSFER();
            case PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT.MessageType: return new PIMP_PLAYER_BECAME_OBSERVER_BANKRUPT();
            case PIMP_PLAYER_WON.MessageType: return new PIMP_PLAYER_WON();
            case PIMP_REQUEST_STATE.MessageType: return new PIMP_REQUEST_STATE();
            case PIMP_STATE_BOARD.MessageType: return new PIMP_STATE_BOARD();
            case PIMP_STATE_PLAYER.MessageType: return new PIMP_STATE_PLAYER();
            case PIMP_STATE_OBSERVER.MessageType: return new PIMP_STATE_OBSERVER();
            case PIMP_STATE_PROPERTY.MessageType: return new PIMP_STATE_PROPERTY();
            case PIMP_STATE_CARD.MessageType: return new PIMP_STATE_CARD();
            case PIMP_STATE_POT.MessageType: return new PIMP_STATE_POT();
            case PIMP_START_OF_TURN.MessageType: return new PIMP_START_OF_TURN();
            case PIMP_THROW_DICE.MessageType: return new PIMP_THROW_DICE();
            case PIMP_DICE_ROLLED.MessageType: return new PIMP_DICE_ROLLED();
            case PIMP_THREE_DOUBLES.MessageType: return new PIMP_THREE_DOUBLES();
            case PIMP_DICE_MOVED_PLAYER.MessageType: return new PIMP_DICE_MOVED_PLAYER();
            case PIMP_CARD_MOVED_PLAYER.MessageType: return new PIMP_CARD_MOVED_PLAYER();
            case PIMP_SQUARE_MOVED_PLAYER.MessageType: return new PIMP_SQUARE_MOVED_PLAYER();
            case PIMP_PLAYER_PASSING_BY_SQUARE.MessageType: return new PIMP_PLAYER_PASSING_BY_SQUARE();
            case PIMP_PLAYER_LANDING_ON_SQUARE.MessageType: return new PIMP_PLAYER_LANDING_ON_SQUARE();
            case PIMP_GOING_TO_JAIL.MessageType: return new PIMP_GOING_TO_JAIL();
            case PIMP_WAITING_FOR_TRANSACTION.MessageType: return new PIMP_WAITING_FOR_TRANSACTION();
            case PIMP_CLAIM_RENT.MessageType: return new PIMP_CLAIM_RENT();
            case PIMP_ERROR_INVALID_RENT_CLAIM.MessageType: return new PIMP_ERROR_INVALID_RENT_CLAIM();
            case PIMP_CLAIM_GO.MessageType: return new PIMP_CLAIM_GO();
            case PIMP_ERROR_INVALID_GO_CLAIM.MessageType: return new PIMP_ERROR_INVALID_GO_CLAIM();
            case PIMP_MONEY_BEING_HELD_IN_ESCROW.MessageType: return new PIMP_MONEY_BEING_HELD_IN_ESCROW();
            case PIMP_RENT_COLLECTION_MORATORIUM.MessageType: return new PIMP_RENT_COLLECTION_MORATORIUM();
            case PIMP_ROLL_AGAIN.MessageType: return new PIMP_ROLL_AGAIN();
            case PIMP_PROPERTY_SALE.MessageType: return new PIMP_PROPERTY_SALE();
            case PIMP_BUY_PROPERTY.MessageType: return new PIMP_BUY_PROPERTY();
            case PIMP_ERROR_PROPERTY_TOO_EXPENSIVE.MessageType: return new PIMP_ERROR_PROPERTY_TOO_EXPENSIVE();
            case PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH.MessageType: return new PIMP_ERROR_PROPERTY_WOULD_REDUCE_NET_WORTH();
            case PIMP_AUCTION_PROPERTY.MessageType: return new PIMP_AUCTION_PROPERTY();
            case PIMP_PROPERTY_AUCTION.MessageType: return new PIMP_PROPERTY_AUCTION();
            case PIMP_BID.MessageType: return new PIMP_BID();
            case PIMP_PROPERTY_AUCTION_BID.MessageType: return new PIMP_PROPERTY_AUCTION_BID();
            case PIMP_NO_BID.MessageType: return new PIMP_NO_BID();
            case PIMP_PROPERTY_AUCTION_NO_BID.MessageType: return new PIMP_PROPERTY_AUCTION_NO_BID();
            case PIMP_PROPERTY_AUCTION_WON.MessageType: return new PIMP_PROPERTY_AUCTION_WON();
            case PIMP_PROPERTY_AUCTION_VOID.MessageType: return new PIMP_PROPERTY_AUCTION_VOID();
            case PIMP_PURCHASE_HOUSES.MessageType: return new PIMP_PURCHASE_HOUSES();
            case PIMP_ERROR_HOUSES_NOT_OWNED.MessageType: return new PIMP_ERROR_HOUSES_NOT_OWNED();
            case PIMP_ERROR_HOUSES_NEED_MONOPOLY.MessageType: return new PIMP_ERROR_HOUSES_NEED_MONOPOLY();
            case PIMP_ERROR_HOUSES_MORTGAGED.MessageType: return new PIMP_ERROR_HOUSES_MORTGAGED();
            case PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP.MessageType: return new PIMP_ERROR_HOUSES_NOT_COLOUR_GROUP();
            case PIMP_ERROR_HOUSES_NOT_YOUR_TURN.MessageType: return new PIMP_ERROR_HOUSES_NOT_YOUR_TURN();
            case PIMP_ERROR_HOUSES_TOO_EXPENSIVE.MessageType: return new PIMP_ERROR_HOUSES_TOO_EXPENSIVE();
            case PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT.MessageType: return new PIMP_ERROR_HOUSES_NO_HOUSE_PIECES_LEFT();
            case PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER.MessageType: return new PIMP_ERROR_HOUSES_CANNOT_BUILD_THAT_NUMBER();
            case PIMP_ERROR_HOUSES_UNBALANCED.MessageType: return new PIMP_ERROR_HOUSES_UNBALANCED();
            case PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH.MessageType: return new PIMP_ERROR_HOUSES_WOULD_REDUCE_NET_WORTH();
            case PIMP_ERROR_HOUSES_TERMITES.MessageType: return new PIMP_ERROR_HOUSES_TERMITES();
            case PIMP_DELTA_HOUSES_PURCHASED.MessageType: return new PIMP_DELTA_HOUSES_PURCHASED();
            case PIMP_MORTGAGE_PROPERTY.MessageType: return new PIMP_MORTGAGE_PROPERTY();
            case PIMP_DELTA_PROPERTY_MORTGAGED.MessageType: return new PIMP_DELTA_PROPERTY_MORTGAGED();
            case PIMP_UNMORTGAGE_PROPERTY.MessageType: return new PIMP_UNMORTGAGE_PROPERTY();
            case PIMP_DELTA_PROPERTY_UNMORTGAGED.MessageType: return new PIMP_DELTA_PROPERTY_UNMORTGAGED();
            case PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE.MessageType: return new PIMP_ERROR_MORTGAGE_TOO_EXPENSIVE();
            case PIMP_GOT_CARD.MessageType: return new PIMP_GOT_CARD();
            case PIMP_CARD_COLLECT_OR_RETRY.MessageType: return new PIMP_CARD_COLLECT_OR_RETRY();
            case PIMP_CARD_SELECT_COLLECT.MessageType: return new PIMP_CARD_SELECT_COLLECT();
            case PIMP_CARD_SELECT_RETRY.MessageType: return new PIMP_CARD_SELECT_RETRY();
            case PIMP_TAX_SELECT_OPTION.MessageType: return new PIMP_TAX_SELECT_OPTION();
            case PIMP_TAX_PAY_TEN_PERCENT.MessageType: return new PIMP_TAX_PAY_TEN_PERCENT();
            case PIMP_TAX_PAY_FLAT_FEE.MessageType: return new PIMP_TAX_PAY_FLAT_FEE();
            case PIMP_JAIL_PAY_OR_ROLL.MessageType: return new PIMP_JAIL_PAY_OR_ROLL();
            case PIMP_JAIL_PAY_BAIL.MessageType: return new PIMP_JAIL_PAY_BAIL();
            case PIMP_JAIL_ROLL_DICE.MessageType: return new PIMP_JAIL_ROLL_DICE();
            case PIMP_JAIL_FREE.MessageType: return new PIMP_JAIL_FREE();
            case PIMP_TRANSACTION_REQUEST_TRADE.MessageType: return new PIMP_TRANSACTION_REQUEST_TRADE();
            case PIMP_TRANSACTION_TRADE_REQUESTED.MessageType: return new PIMP_TRANSACTION_TRADE_REQUESTED();
            case PIMP_TRANSACTION_RENT_REQUESTED.MessageType: return new PIMP_TRANSACTION_RENT_REQUESTED();
            case PIMP_TRANSACTION_CARD_REQUESTED.MessageType: return new PIMP_TRANSACTION_CARD_REQUESTED();
            case PIMP_TRANSACTION_SQUARE_REQUESTED.MessageType: return new PIMP_TRANSACTION_SQUARE_REQUESTED();
            case PIMP_TRANSACTION_BANK_REQUESTED.MessageType: return new PIMP_TRANSACTION_BANK_REQUESTED();
            case PIMP_TRANSACTION_JAIL_REQUESTED.MessageType: return new PIMP_TRANSACTION_JAIL_REQUESTED();
            case PIMP_TRANSACTION_SET_CASH.MessageType: return new PIMP_TRANSACTION_SET_CASH();
            case PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE.MessageType: return new PIMP_ERROR_TRANSACTION_TOO_EXPENSIVE();
            case PIMP_TRANSACTION_CASH_SET.MessageType: return new PIMP_TRANSACTION_CASH_SET();
            case PIMP_TRANSACTION_OTHER_CASH_SET.MessageType: return new PIMP_TRANSACTION_OTHER_CASH_SET();
            case PIMP_TRANSACTION_ADD_PROPERTY.MessageType: return new PIMP_TRANSACTION_ADD_PROPERTY();
            case PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED.MessageType: return new PIMP_ERROR_TRANSACTION_PROPERTY_NOT_OWNED();
            case PIMP_TRANSACTION_REMOVE_PROPERTY.MessageType: return new PIMP_TRANSACTION_REMOVE_PROPERTY();
            case PIMP_TRANSACTION_PROPERTY_ADDED.MessageType: return new PIMP_TRANSACTION_PROPERTY_ADDED();
            case PIMP_TRANSACTION_PROPERTY_REMOVED.MessageType: return new PIMP_TRANSACTION_PROPERTY_REMOVED();
            case PIMP_TRANSACTION_OTHER_PROPERTY_ADDED.MessageType: return new PIMP_TRANSACTION_OTHER_PROPERTY_ADDED();
            case PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED.MessageType: return new PIMP_TRANSACTION_OTHER_PROPERTY_REMOVED();
            case PIMP_TRANSACTION_ADD_CARD.MessageType: return new PIMP_TRANSACTION_ADD_CARD();
            case PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED.MessageType: return new PIMP_ERROR_TRANSACTION_CARD_NOT_OWNED();
            case PIMP_TRANSACTION_REMOVE_CARD.MessageType: return new PIMP_TRANSACTION_REMOVE_CARD();
            case PIMP_TRANSACTION_CARD_ADDED.MessageType: return new PIMP_TRANSACTION_CARD_ADDED();
            case PIMP_TRANSACTION_CARD_REMOVED.MessageType: return new PIMP_TRANSACTION_CARD_REMOVED();
            case PIMP_TRANSACTION_OTHER_CARD_ADDED.MessageType: return new PIMP_TRANSACTION_OTHER_CARD_ADDED();
            case PIMP_TRANSACTION_OTHER_CARD_REMOVED.MessageType: return new PIMP_TRANSACTION_OTHER_CARD_REMOVED();
            case PIMP_TRANSACTION_FINISH.MessageType: return new PIMP_TRANSACTION_FINISH();
            case PIMP_TRANSACTION_FINISHED.MessageType: return new PIMP_TRANSACTION_FINISHED();
            case PIMP_TRANSACTION_OTHER_FINISHED.MessageType: return new PIMP_TRANSACTION_OTHER_FINISHED();
            case PIMP_ERROR_TRANSACTION_NOT_SUITABLE.MessageType: return new PIMP_ERROR_TRANSACTION_NOT_SUITABLE();
            case PIMP_TRANSACTION_REOPEN.MessageType: return new PIMP_TRANSACTION_REOPEN();
            case PIMP_TRANSACTION_REOPENED.MessageType: return new PIMP_TRANSACTION_REOPENED();
            case PIMP_TRANSACTION_OTHER_REOPENED.MessageType: return new PIMP_TRANSACTION_OTHER_REOPENED();
            case PIMP_TRANSACTION_AGREE.MessageType: return new PIMP_TRANSACTION_AGREE();
            case PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE.MessageType: return new PIMP_ERROR_TRANSACTION_PROPERTY_HAS_HOUSE();
            case PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH.MessageType: return new PIMP_ERROR_TRANSACTION_WOULD_REDUCE_NET_WORTH();
            case PIMP_TRANSACTION_AGREED.MessageType: return new PIMP_TRANSACTION_AGREED();
            case PIMP_TRANSACTION_OTHER_AGREED.MessageType: return new PIMP_TRANSACTION_OTHER_AGREED();
            case PIMP_TRANSACTION_FINALISED.MessageType: return new PIMP_TRANSACTION_FINALISED();
            case PIMP_BANKRUPT_TRANSACTION.MessageType: return new PIMP_BANKRUPT_TRANSACTION();
            case PIMP_ERROR_TRANSACTION_NOT_BANKRUPT.MessageType: return new PIMP_ERROR_TRANSACTION_NOT_BANKRUPT();
            case PIMP_TRANSACTION_UNUSUAL.MessageType: return new PIMP_TRANSACTION_UNUSUAL();
            case PIMP_TRANSACTION_CANCEL.MessageType: return new PIMP_TRANSACTION_CANCEL();
            case PIMP_TRANSACTION_CANCELLED.MessageType: return new PIMP_TRANSACTION_CANCELLED();
            case PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED.MessageType: return new PIMP_ERROR_TRANSACTION_CANNOT_BE_CANCELLED();
            case PIMP_SQUARE_GIVES_CASH.MessageType: return new PIMP_SQUARE_GIVES_CASH();
            case PIMP_SQUARE_TAKES_CASH.MessageType: return new PIMP_SQUARE_TAKES_CASH();
            case PIMP_SQUARE_GIVES_CARD.MessageType: return new PIMP_SQUARE_GIVES_CARD();
            case PIMP_SQUARE_TAKES_CARD.MessageType: return new PIMP_SQUARE_TAKES_CARD();
            case PIMP_CONSTRUCTION_TAKES_CASH.MessageType: return new PIMP_CONSTRUCTION_TAKES_CASH();
            case PIMP_DESTRUCTION_GIVES_CASH.MessageType: return new PIMP_DESTRUCTION_GIVES_CASH();
            case PIMP_PLAYER_CLAIMED_RENT.MessageType: return new PIMP_PLAYER_CLAIMED_RENT();
            case PIMP_PLAYER_CLAIMED_GO.MessageType: return new PIMP_PLAYER_CLAIMED_GO();
            case PIMP_DELTA_CASH.MessageType: return new PIMP_DELTA_CASH();
            case PIMP_DELTA_PROPERTY.MessageType: return new PIMP_DELTA_PROPERTY();
            case PIMP_DELTA_CARD.MessageType: return new PIMP_DELTA_CARD();
            case PIMP_DELTA_BANK.MessageType: return new PIMP_DELTA_BANK();
            case PIMP_DELTA_POT.MessageType: return new PIMP_DELTA_POT();
          default: throw new UnexpectedMessageException();
        }
    }
}
