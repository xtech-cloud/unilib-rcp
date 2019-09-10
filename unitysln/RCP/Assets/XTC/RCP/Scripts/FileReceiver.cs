using System;
using System.IO;
using System.Text;

namespace XTC.RCP
{
    public class FileReceiver : StreamReceiver
    {
        public string directory { get; set; }

        protected override void handleClientConnected (Receiver.AsyncEventArgs _args)
        {
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(directory);

            _args.state.writer.guid = Guid.NewGuid().ToString();
            string file = Path.Combine(directory, _args.state.writer.guid + ".tmp");
            _args.state.writer.stream = File.Open(file, FileMode.Create, FileAccess.Write);
        }

        protected override void handleClientDisconnected(Receiver.AsyncEventArgs _args)
        {
            _args.state.writer.stream.Flush();
            bool isComplete = _args.state.writer.stream.Length + Header.Size == _args.state.writer.size;
            _args.state.writer.stream.Close();
            _args.state.writer.stream.Dispose();

            string tmpFile =Path.Combine(directory, _args.state.writer.guid + ".tmp");
            string distFile =Path.Combine(directory, _args.state.writer.filename);
            if(isComplete)
                File.Move(tmpFile, distFile);
            else
                File.Delete(tmpFile);
        }
    }
}