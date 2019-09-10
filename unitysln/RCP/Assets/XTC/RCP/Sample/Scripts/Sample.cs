using System;
using System.IO;
using System.Collections;
using UnityEngine;
using XTC.RCP;

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    private FileReceiver fileReceiver;
    private MemoryReceiver memoryReceiver;
    IEnumerator Start()
    {
        fileReceiver = new FileReceiver();
        fileReceiver.directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        fileReceiver.Listen(Receiver.DefaultPort);

        fileReceiver.OnReady += OnReady;
        fileReceiver.OnReceive += OnReceive;
        fileReceiver.OnFinish += OnFinish;
        fileReceiver.OnError += OnError;
        fileReceiver.OnException += OnException;

        yield return new WaitForSeconds(1);
        memoryReceiver = new MemoryReceiver();
        memoryReceiver.Listen(Receiver.DefaultPort + 10);

        yield return new WaitForSeconds(1);
        string dir = Path.Combine(Application.dataPath, "XTC/RCP/Scripts");
        foreach (string file in Directory.GetFiles(dir))
        {
            string filename = Path.GetFileName(file);
            byte[] data = File.ReadAllBytes(file);
            MemoryStream mStream = new MemoryStream(data);
            Sender sender = new Sender();
            sender.Send("127.0.0.1", Receiver.DefaultPort, mStream, filename);
            //sender.Send("127.0.0.1", Receiver.DefaultPort + 1, mStream, filename);
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("-----------------");
        yield return new WaitForSeconds(1);
        //big file
        {
            // 2M
            byte[] data = new byte[1024*1024*2];
            MemoryStream mStream = new MemoryStream(data);
            Sender sender = new Sender();
            sender.Send("127.0.0.1", Receiver.DefaultPort, mStream, "1.bin");
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Stop receiver");
        fileReceiver.Stop();
        memoryReceiver.Stop();
    }

    private void OnReady(object _sender, StreamReceiver.AsyncEventArgs _args)
    {
        Debug.Log("ready receive");;
    }

    private void OnReceive(object _sender, StreamReceiver.AsyncEventArgs _args)
    {
        Debug.Log(string.Format(">> receive {0}/{1}", _args.stream.Length, _args.size));
    }

    private void OnFinish(object _sender, StreamReceiver.AsyncEventArgs _args)
    {
        Debug.Log(string.Format("receive finish {0}/{1}", _args.stream.Length, _args.size));
    }

    private void OnError(object _sender, StreamReceiver.AsyncEventArgs _args)
    {
        Debug.LogError( _args.message);
    }

    private void OnException(object _sender, StreamReceiver.AsyncEventArgs _args)
    {
        Debug.LogError( _args.message);
    }

}
