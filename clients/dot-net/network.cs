// -*- Mode: Java -*-

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Windows.Forms;

// An encapsulation of the Sockets class that implements the PIMP protocol
public class Network : IDisposable {

    private Socket socket;
    private NetworkStream stream;
    protected String host;
    protected int port;
    protected Thread thread;
    public Thread Thread { get { return thread; } }
    protected Control ownerControl;

    public Network(String aHost, int aPort, Control aOwnerControl) {
        host = aHost;
        port = aPort;
        socket = null;
        stream = null;
        ownerControl = aOwnerControl;
        thread = new Thread(new ThreadStart(ThreadCode));
    }

    ~Network() {
        Dispose();
    }

    public void Start() {
        thread.Start();
    }

    public void Dispose() {
        GC.SuppressFinalize(this);
        thread.Abort(); // kill our child, hee hee :-D
        Disconnect();
        thread.Join(); // wait for it to be done with our objects
    }

    protected void Disconnect() {
        if (stream != null) {
            stream.Close();
            stream = null;
        }
        if (socket != null) {
            socket.Close();
            socket = null;
        }
    }

    public void SendMessage(Message message) {
        if (stream != null) {
            lock (stream) {
                // Send the data and flush it out
                int length;
                byte[] buffer = message.Serialize(out length);
                stream.Write(buffer, 0, length);
                stream.Flush();
            }
        } // else the player will have to ask us to do whatever it was again
    }

    protected void ProcessMessage(Message message) {
        lock (this) {
            switch (message.GetMessageType()) {
            case PIMP_ERROR_UNEXPECTED_MESSAGE.MessageType:
                OnNetworkError(new StringEventArgs("We sent a message that the server was not expecting"));
                break;
            case PIMP_ERROR_INVALID_PAYLOAD.MessageType:
                OnNetworkError(new StringEventArgs("We sent a message with an invalid payload"));
                break;
            case PIMP_ERROR_UNPARSEABLE.MessageType:
                OnNetworkError(new StringEventArgs("We sent an unparseable message"));
                break;
            case PIMP_ERROR_UNKNOWN_PROTOCOL.MessageType:
                OnNetworkFatalError(new StringEventArgs("The server does not speak our protocol"));
                break;
            default:
                OnReceiveMessage(new ReceiveMessageEventArgs(message));
                break;
            }
        }
    }

    // Establish a socket connection and start receiving
    protected void ThreadCode() {

        // The structure of this function is simple: it's an infinite
        // loop nested in an infinite loop.
        // 
        // The outer infinite loop consists of a block of code that
        // connects to the remote server, labelled "connect". That is
        // then followed by a loop that tries to read data from this
        // server forever.
        //
        // If anything happens at any point, the statement "goto
        // connect" leads us straight back to the top.
        //
        // The only way out of this function is for the thread to be
        // aborted externally.

        while (true) {

            connect:
            try {
                IPEndPoint address = new IPEndPoint(Dns.Resolve(host).AddressList[0], port);
                socket = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream, ProtocolType.Tcp);
                socket.Blocking = true;
                socket.Connect(address);
                socket.SetSocketOption(SocketOptionLevel.Socket,
                                       SocketOptionName.ReceiveTimeout, 0);
                socket.SetSocketOption(SocketOptionLevel.Socket,
                                       SocketOptionName.SendTimeout, 5000);
                stream = new NetworkStream(socket);
            } catch (Exception e) {
                OnNetworkFatalError(new StringEventArgs(e.Message));
                Disconnect();
                goto connect;
            }

            // send handshake
            OnNetworkConnected(new EventArgs());
            SendMessage(new PIMP_HANDSHAKE(1));

            // try to get data
            try {
                byte[] dataStream = new byte[Message.MAX_LENGTH];
                int length = 0;
                while (true) {
                    // Note: In theory, on Win2k and later, the
                    // ReadByte() method will generate an exception
                    // with socket error code 10054 when the remote
                    // connection closes, and on Win9x platforms, it
                    // will simply return 0. In practice, it seems
                    // that |stream| gets set to |null| and we get a
                    // NullReferenceException. So we cover all
                    // possible scenarios here.
                    int data;
                    try {
                        data = stream.ReadByte();
                    } catch (NullReferenceException) {
                        data = -1; // we were disconnected
                    } catch (IOException e1) {
                        SocketException e2 = e1.InnerException as SocketException;
                        if (e2 != null && e2.ErrorCode == 10054) {
                            data = -1; // we were disconnected
                        } else {
                            throw; // something else, catch it below
                        }
                    }
                    if (data < 0) {
                        // connection was closed somehow
                        OnNetworkDisconnected(new EventArgs());
                        Disconnect();
                        goto connect;
                    }
                    // have data 
                    // add the data to the buffer
                    dataStream[length++] = Convert.ToByte(data);
                    // Check to see if we have a complete message...
                    if ((length >= 2) && (length >= dataStream[1] + 2)) {
                        // If so, parse it and throw it to the main thread.
                        // Note: dataStream[1] will always be in the range 0..255,
                        // so length cannot exceed 257, the size of the array.
                        try {
                            byte[] payload = new byte[Message.MAX_LENGTH];
                            Array.Copy(dataStream, 2, payload, 0, length - 2);
                            ProcessMessage(Message.Parse(payload, dataStream[0]));
                        } catch(UnexpectedMessageException) {
                            OnNetworkError(new StringEventArgs("We received an unrecognised message"));
                            SendMessage(new PIMP_ERROR_UNEXPECTED_MESSAGE(dataStream[0]));
                        } catch(IndexOutOfRangeException) {
                            OnNetworkError(new StringEventArgs("We received a message that we could not parse"));
                            SendMessage(new PIMP_ERROR_INVALID_PAYLOAD(dataStream[0]));
                        }
                        // now we're done.
                        // and since we're reading this one byte at a
                        // time, we know that there is no more data
                        // that isn't from this message, so reset the
                        // buffer.
                        length = 0;
                    }
                }
            } catch (ThreadAbortException) {
                // never mind
            } catch (Exception e) {
                // eek
                OnNetworkError(new StringEventArgs(e.Message));
                OnNetworkDisconnected(new EventArgs());
                Disconnect();
                goto connect;
            }
        }
    }

    public delegate void ReceiveMessageEventHandler(Object sender, ReceiveMessageEventArgs e);
    public class ReceiveMessageEventArgs : EventArgs {
        private readonly Message message;
        public Message Message { get { return message; } }
        public ReceiveMessageEventArgs(Message aMessage) : base() {
            message = aMessage;
        }
    }

    public event ReceiveMessageEventHandler ReceiveMessage;
    protected virtual void OnReceiveMessage(ReceiveMessageEventArgs e) {
        lock (this) {
            if (ReceiveMessage != null) {
                ownerControl.BeginInvoke(ReceiveMessage, new Object[] {this, e});
            }
        }
    }

    public delegate void StringEventHandler(Object sender, StringEventArgs e);
    public class StringEventArgs : EventArgs {
        private readonly String data;
        public String Data { get { return data; } }
        public StringEventArgs(String aData) : base() {
            data = aData;
        }
    }

    public event StringEventHandler NetworkError;
    protected virtual void OnNetworkError(StringEventArgs e) {
        lock (this) {
            if (NetworkError != null) {
                ownerControl.BeginInvoke(NetworkError, new Object[] {this, e});
            }
        }
    }

    public event StringEventHandler NetworkFatalError;
    protected virtual void OnNetworkFatalError(StringEventArgs e) {
        lock (this) {
            if (NetworkFatalError != null) {
                ownerControl.BeginInvoke(NetworkFatalError, new Object[] {this, e});
            }
        }
    }

    public event EventHandler NetworkDisconnected;
    protected virtual void OnNetworkDisconnected(EventArgs e) {
        lock (this) {
            if (NetworkDisconnected != null) {
                ownerControl.BeginInvoke(NetworkDisconnected, new Object[] {this, e});
            }
        }
    }

    public event EventHandler NetworkConnected;
    protected virtual void OnNetworkConnected(EventArgs e) {
        lock (this) {
            if (NetworkConnected != null) {
                ownerControl.BeginInvoke(NetworkConnected, new Object[] {this, e});
            }
        }
    }

}
