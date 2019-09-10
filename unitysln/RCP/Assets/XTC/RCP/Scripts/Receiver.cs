using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace XTC.RCP
{
    public class Receiver
    {
        public class AsyncEventArgs : EventArgs
        {
            public string msg { get; private set; }

            public ClientState state { get; private set; }

            public bool IsHandled { get; private set; }

            public AsyncEventArgs(string _msg)
            {
                msg = _msg;
                IsHandled = false;
            }
            public AsyncEventArgs(ClientState _state)
            {
                state = _state;
                IsHandled = false;
            }
            public AsyncEventArgs(string _msg, ClientState _state)
            {
                msg = _msg;
                state = _state;
                IsHandled = false;
            }
        }

        public const int DefaultPort = 15654;

        public bool IsRunning { get; private set; }

        public event EventHandler<AsyncEventArgs> ClientConnected;
        public event EventHandler<AsyncEventArgs> ClientDisconnected;
        public event EventHandler<AsyncEventArgs> DataReceived;
        public event EventHandler<AsyncEventArgs> NetError;
        public event EventHandler<AsyncEventArgs> Exception;

        private TcpListener listener_ = null;

        private List<Object> clients_;

        public void Listen(int _port)
        {
            if (IsRunning)
                return;

            IsRunning = true;

            clients_ = new List<Object>();
            listener_ = new TcpListener(IPAddress.Any, _port);
            listener_.Start();
            listener_.BeginAcceptTcpClient(new AsyncCallback(handleAccepted), listener_);
        }

        public void Listen(int _port, int _backlog)
        {
            if (IsRunning)
                return;

            IsRunning = true;

            clients_ = new List<Object>();
            listener_ = new TcpListener(IPAddress.Any, _port);
            listener_.Start(_backlog);
            listener_.BeginAcceptTcpClient(new AsyncCallback(handleAccepted), listener_);
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            listener_.Stop();
            lock (clients_)
            {
                closeAllClient();
            }
        }

        private void handleAccepted(IAsyncResult _ar)
        {
            TcpListener listen = (TcpListener)_ar.AsyncState;
            TcpClient client = listen.EndAcceptTcpClient(_ar);
            byte[] buffer = new byte[client.ReceiveBufferSize];
            ClientState state = new ClientState(client, buffer);
            lock (clients_)
            {
                clients_.Add(state);
                raiseClientConnected(state);
            }

            NetworkStream stream = state.NetworkStream;
            
            stream.BeginRead(state.Buffer, 0, state.Buffer.Length, handleDataReceived, state);

            listen.BeginAcceptTcpClient(new AsyncCallback(handleAccepted), _ar.AsyncState);
        }

        private void handleDataReceived(IAsyncResult _ar)
        {
            if (!IsRunning)
                return;

            ClientState state = (ClientState)_ar.AsyncState;
            NetworkStream stream = state.NetworkStream;

            int recv = 0;
            try
            {
                recv = stream.EndRead(_ar);
            }
            catch(Exception ex)
            {
                raiseOtherException(state, ex.Message);
                recv = 0;
            }

            if (recv == 0)
            {
                lock (clients_)
                {
                    clients_.Remove(state);
                    raiseClientDisconnected(state);
                    return;
                }
            }
            state.currentBlockSize = recv;
            raiseDataReceived(state);
            stream.BeginRead(state.Buffer, 0, state.Buffer.Length, handleDataReceived, state);
        }

        private void closeAllClient()
        {
            foreach (ClientState client in clients_)
            {
                close(client);
            }
            clients_.Clear();
        }

        private void raiseClientConnected(ClientState state)
        {
            if (ClientConnected != null)
            {
                ClientConnected(this, new AsyncEventArgs(state));
            }
        }

        private void raiseClientDisconnected(ClientState state)
        {
            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, new AsyncEventArgs(state));
            }
        }

        private void raiseNetError(ClientState state)
        {
            if (NetError != null)
            {
                NetError(this, new AsyncEventArgs(state));
            }
        }

        private void raiseDataReceived(ClientState state)
        {
            //UnityEngine.Debug.Log("raiseDataReceived");
            if (DataReceived != null)
            {
                DataReceived(this, new AsyncEventArgs(state));
            }
        }

        private void raiseOtherException(ClientState state, string descrip)
        {
            if (Exception != null)
            {
                Exception(this, new AsyncEventArgs(descrip, state));
            }
        }
        private void raiseOtherException(ClientState state)
        {
            raiseOtherException(state, "");
        }

        private void close(ClientState state)
        {
            if (state != null)
            {
                state.Close();
                clients_.Remove(state);
            }
        }

        
    }
}