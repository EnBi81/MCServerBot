using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCServerHandler
{
    internal class FileChecker
    {
        private const string AliveFileName = "server-running.file";
        public string Directory { get; }

        private bool CheckFile { get; set; }
        private Thread CheckThread { get; set; }

        public FileChecker(string directory)
        {
            if (!directory.EndsWith("/") && !directory.EndsWith("\\"))
                directory += "\\";

            Directory = directory;
        }

        public void Start()
        {
            CreateAliveFile();
            CheckThread = new Thread(CheckFileConstantly);
            CheckThread.Start();
        }

        public void Stop()
        {
            CheckFile = false;
            File.Delete(Directory + AliveFileName);
        }

        private void CheckFileConstantly()
        {
            CheckFile = true;
            while (CheckFile)
            {
                if (!CheckFileExists())
                {
                    FileDeleted?.Invoke(this, new object());
                    return;
                }

                Thread.Sleep(10_000);
            }
        } 

        private bool CreateAliveFile()
        {
            if (CheckFileExists())
                return false;

            try
            {
                File.WriteAllText(Directory + AliveFileName, "");
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool CheckFileExists() => File.Exists(Directory + AliveFileName);

        public event EventHandler<object> FileDeleted;
    }
}
