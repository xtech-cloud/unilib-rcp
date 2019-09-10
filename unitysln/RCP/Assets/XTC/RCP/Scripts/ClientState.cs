using System;
using System.IO;
using System.Net.Sockets;

namespace XTC.RCP
{
    public class Writer
    {
        public string guid = "";
        public string filename = "";
        public long size = 0;
        public Stream stream {get;set;}
    }

    public class ClientState
    {
        public TcpClient TcpClient { get; private set; }

        public byte[] Buffer { get; private set; }

        public int currentBlockSize {get;set;}
        public int receivedSize {get;set;}

        public Writer writer {get;set;}

        public NetworkStream NetworkStream
        {
            get { return TcpClient.GetStream(); }
        }

        public ClientState(TcpClient _tcpClient, byte[] _buffer)
        {
            if (_tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            if (_buffer == null)
                throw new ArgumentNullException("buffer");

            this.TcpClient = _tcpClient;
            this.Buffer = _buffer;
            this.currentBlockSize =0;
            this.receivedSize = 0;
            this.writer = new Writer();
        }

        public void Close()
        {
            TcpClient.Close();
            Buffer = null;
        }
    }
}