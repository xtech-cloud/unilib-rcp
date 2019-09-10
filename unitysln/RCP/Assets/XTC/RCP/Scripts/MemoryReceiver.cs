using UnityEngine;
using System;
using System.Text;
using System.IO;

namespace XTC.RCP
{
    public class MemoryReceiver : StreamReceiver
    {
        protected override void handleClientConnected (Receiver.AsyncEventArgs _args)
        {
            _args.state.writer.guid = Guid.NewGuid().ToString();
            _args.state.writer.stream = new MemoryStream();
        }

        protected override void handleClientDisconnected(Receiver.AsyncEventArgs _args)
        {
            _args.state.writer.stream.Flush();
            _args.state.writer.stream.Close();
            _args.state.writer.stream.Dispose();
            _args.state.writer = null;

        }
    }
}