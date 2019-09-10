using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace XTC.RCP
{
    public class Sender
    {
        public class State
        {
            public NetworkStream nwStream;
            public Stream stream;
            public byte[] buffer;
        }

        public void Send(string _remoteIP, int _remotePort, Stream _stream, string _name)
        {
            if(_name.Length > 128)
                throw new System.Exception("name must less 128 characters");
                
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(_remoteIP), _remotePort);
            TcpClient client = new TcpClient();
            client.Connect(remoteEndPoint);
            
            State state = new State();
            state.nwStream = client.GetStream();
            state.stream = _stream;
            state.buffer = new byte[client.SendBufferSize];

            byte[] totalSize = Convert.Int64ToBytes( Header.Size + _stream.Length);
            state.nwStream.Write(totalSize, 0, totalSize.Length);
            byte[] length = Convert.Int32ToBytes(_name.Length);
            state.nwStream.Write(length, 0, length.Length);
            byte[] nameBuffer = new byte[128*4];
            byte[] nameData = Encoding.UTF8.GetBytes(_name);
            Buffer.BlockCopy(nameData, 0, nameBuffer, 0, Math.Min(nameBuffer.Length, nameData.Length));
            state.nwStream.Write(nameBuffer, 0, nameBuffer.Length);

            _stream.BeginRead(state.buffer, 0, state.buffer.Length, new AsyncCallback(handleDataSended), state);
        }

        private void handleDataSended(IAsyncResult _ar)
        {
            State state = (State)_ar.AsyncState;

            int recv = state.stream.EndRead(_ar);
            if(0 == recv)
            {
                state.stream.Close();
                state.nwStream.Close();
                return;
            }
            state.nwStream.Write(state.buffer, 0, recv);
            state.stream.BeginRead(state.buffer, 0, state.buffer.Length, new AsyncCallback(handleDataSended), state);
        }
    }
}