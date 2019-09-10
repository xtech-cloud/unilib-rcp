using UnityEngine;
using System;
using System.Text;
using System.IO;

namespace XTC.RCP
{
    public abstract class StreamReceiver
    {
        public class AsyncEventArgs : EventArgs
        {
            public string filename { get; private set; }
            public string message { get; private set; }
            public long size { get; private set; }
            public Stream stream {get; private set;}

            public AsyncEventArgs(string _filename, long _size, string _message, Stream _stream)
            {
                message = _message;
                size = _size;
                filename = _filename;
                stream = _stream;
            }
        }

        public event EventHandler<AsyncEventArgs> OnFinish;
        public event EventHandler<AsyncEventArgs> OnReady;
        public event EventHandler<AsyncEventArgs> OnReceive;
        public event EventHandler<AsyncEventArgs> OnException;
        public event EventHandler<AsyncEventArgs> OnError;

        protected Receiver receiver { get; set; }

        public void Listen(int _port)
        {
            receiver = new Receiver();
            receiver.ClientConnected += onClientConnected;
            receiver.ClientDisconnected += onClientDisconnected;
            receiver.NetError += onNetError;
            receiver.Exception += onException;
            receiver.DataReceived += onDataReceived;
            receiver.Listen(_port);
        }

        public void Stop()
        {
            receiver.Stop();
        }

        protected abstract void handleClientConnected(Receiver.AsyncEventArgs _args);
        protected abstract void handleClientDisconnected(Receiver.AsyncEventArgs _args);

        void onClientConnected(object _sender, Receiver.AsyncEventArgs _args)
        {
            handleClientConnected(_args);
            if (null != this.OnReady)
                OnReady(this, newArgs(_args, ""));
        }

        void onClientDisconnected(object _sender, Receiver.AsyncEventArgs _args)
        {
            if (null != OnFinish)
                OnFinish(this, newArgs(_args, ""));
            handleClientDisconnected(_args);
        }

        void onNetError(object _sender, Receiver.AsyncEventArgs _args)
        {
            if (null != OnError)
                OnError(this, newArgs(_args, _args.msg));
        }

        void onException(object _sender, Receiver.AsyncEventArgs _args)
        {
            if (null != OnException)
                OnException(this, newArgs(_args, _args.msg));
        }

        void onDataReceived(object _sender, Receiver.AsyncEventArgs _args)
        {
            int currentBlockSize = _args.state.currentBlockSize;
            int fileBlockSize = currentBlockSize;
            int fileReceivedSize = currentBlockSize;
            int offset = 0;
            if (_args.state.writer.stream.Position == 0)
            {
                if (_args.state.currentBlockSize >= Header.Size)
                {
                    _args.state.writer.size = Convert.BytesToInt64(_args.state.Buffer, 0);
                    int nameLength = Convert.BytesToInt32(_args.state.Buffer, 8);
                    _args.state.writer.filename = Encoding.UTF8.GetString(_args.state.Buffer, 8 + 4, nameLength);
                    fileBlockSize = _args.state.currentBlockSize - Header.Size;
                    fileReceivedSize = _args.state.currentBlockSize - Header.Size;
                    offset = Header.Size;
                }
            }
            _args.state.writer.stream.Write(_args.state.Buffer, offset, fileBlockSize);
            _args.state.receivedSize += fileReceivedSize;

            if (null != OnReceive)
                OnReceive(this, newArgs(_args, ""));
        }

        private AsyncEventArgs newArgs(Receiver.AsyncEventArgs _args, string _message)
        {
            return new AsyncEventArgs(_args.state.writer.filename, _args.state.writer.size - Header.Size, _message, _args.state.writer.stream);
        }
    }


}