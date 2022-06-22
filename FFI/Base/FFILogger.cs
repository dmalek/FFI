using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FFI.Base
{
    public class FFILogger : IFFILogger
    {
        private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
        private readonly object loggerLock = new object();

        public string LogFilePath { get; set; }

        public FFILogger()
        {

        }

        public void Log(string message, params object[] args)
        {
            message = String.Format(message, args);
            WriteLine(message);
        }

        private void WriteLine(string message)
        {            
            _messageQueue.Enqueue(message + Environment.NewLine);
            Task.Run(() => ProcessMessageQueue());
        }

        private void ProcessMessageQueue()
        {
            lock (loggerLock)
            {
                while (_messageQueue.TryDequeue(out string message))
                {
                    Debug.Write(message);
                    Console.Write(message);

                    if (!string.IsNullOrEmpty(LogFilePath))
                    {
                        try
                        {
                            File.AppendAllText(LogFilePath, message, Encoding.UTF8);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Debug.WriteLine(e);
                        }
                    }
                }
            }
        }


    }
}
