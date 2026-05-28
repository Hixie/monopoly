// -*- Mode: Java -*-

using System;

public abstract class Message {

    public const int MAX_LENGTH = 257;

    public abstract byte GetMessageType();

    public static Message Parse(byte[] payload, byte type) {
        Message message = MessageMap.CreateMessage(type);
        message.ParsePayload(ref payload);
        return message;
    }
    
    virtual protected void ParsePayload(ref byte[] payload) { }

    public byte[] Serialize(out int length) {
        byte index = 2;
        byte[] value = new byte[MAX_LENGTH];
        SerializePayload(ref value, ref index);
        length = index;
        index = 0;
        Write(GetMessageType(), ref value, ref index);
        Write(Convert.ToByte(length - 2), ref value, ref index);
        return value;
    }

    virtual protected void SerializePayload(ref byte[] payload, ref byte index) { }


    // Writers

    protected void Write(bool value, ref byte[] output, ref byte index) {
        output[index++] = (byte)(value ? 1 : 0);
    }

    protected void Write(byte value, ref byte[] output, ref byte index) {
        output[index++] = value;
    }

    protected void Write(sbyte value, ref byte[] output, ref byte index) {
        output[index++] = Convert.ToByte(value & 0xFF);
    }

    protected void Write(uint value, ref byte[] output, ref byte index) {
        // network byte order (big end first)
        output[index++] = Convert.ToByte(value >> 8 >> 8 >> 8 & 0xFF);
        output[index++] = Convert.ToByte(value >> 8 >> 8 & 0xFF);
        output[index++] = Convert.ToByte(value >> 8 & 0xFF);
        output[index++] = Convert.ToByte(value & 0xFF);
    }

    protected void Write(int value, ref byte[] output, ref byte index) {
        // convert int representation into uint
        // network byte order (big end first)
        output[index++] = Convert.ToByte(value >> 8 >> 8 >> 8 & 0xFF);
        output[index++] = Convert.ToByte(value >> 8 >> 8 & 0xFF);
        output[index++] = Convert.ToByte(value >> 8 & 0xFF);
        output[index++] = Convert.ToByte(value & 0xFF);
    }

    protected void Write(String value, ref byte[] output, ref byte index) {
        byte length = Convert.ToByte(System.Text.Encoding.UTF8.GetByteCount(value));
        output[index++] = length;
        System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, output, index);
        index += length;
    }


    // Readers

    protected void Read(out bool value, ref byte[] input, ref byte index) {
        value = input[index++] == 1 ? true : false;
    }

    protected void Read(out byte value, ref byte[] input, ref byte index) {
        value = input[index++];
    }

    protected void Read(out sbyte value, ref byte[] input, ref byte index) {
        byte data = input[index++];
        if ((data & 0x80) == 0) {
            // positive
            value = Convert.ToSByte(data & 0x7F);
        } else {
            // negative
            value = Convert.ToSByte(SByte.MaxValue - (data & 0x7F));
        }
    }

    protected void Read(out uint value, ref byte[] input, ref byte index) {
        value = Convert.ToUInt32(input[index++] << 8 << 8 << 8);
        value += Convert.ToUInt32(input[index++] << 8 << 8);
        value += Convert.ToUInt32(input[index++] << 8);
        value += Convert.ToUInt32(input[index++]);
    }

    protected void Read(out int value, ref byte[] input, ref byte index) {
        value = Convert.ToInt32(input[index++] << 8 << 8 << 8);
        value += Convert.ToInt32(input[index++] << 8 << 8);
        value += Convert.ToInt32(input[index++] << 8);
        value += Convert.ToInt32(input[index++]);
    }

    protected void Read(out String value, ref byte[] input, ref byte index) {
        int length = input[index++];
        value = System.Text.Encoding.UTF8.GetString(input, index, length);
        index += Convert.ToByte(length);
    }

}

class NetworkException : ApplicationException {}
class UnexpectedMessageException : NetworkException {}


class PIMP_PURCHASE_HOUSES : Message {
    public const byte MessageType = 0x90;
    override public byte GetMessageType() { return MessageType; }
    public byte Field1;
    public byte[] Field2;
    public sbyte[] Field3;
    public sbyte[] Field4;
    public PIMP_PURCHASE_HOUSES() : base() { }
    public PIMP_PURCHASE_HOUSES(byte[] field2, sbyte[] field3, sbyte[] field4) : base() {
        if (!(field2.Length == field3.Length) && (field2.Length == field4.Length))
            throw new ArgumentException();
        Field1 = Convert.ToByte(field2.Length);
        Field2 = field2;
        Field3 = field3;
        Field4 = field4;
    }
    override protected void SerializePayload(ref byte[] payload, ref byte index) {
        Write(Field1, ref payload, ref index);
        for (int i = 0; i < Field1; ++i) {
            Write(Field2[i], ref payload, ref index);
            Write(Field3[i], ref payload, ref index);
            Write(Field4[i], ref payload, ref index);
        }
    }
    override protected void ParsePayload(ref byte[] payload) {
        byte index = 0;
        Read(out Field1, ref payload, ref index);
        Field2 = new Byte[Field1];
        Field3 = new SByte[Field1];
        Field4 = new SByte[Field1]; 
        for (int i = 0; i < Field1; ++i) {
            Read(out Field2[i], ref payload, ref index);
            Read(out Field3[i], ref payload, ref index);
            Read(out Field4[i], ref payload, ref index);
        }
    }

}
