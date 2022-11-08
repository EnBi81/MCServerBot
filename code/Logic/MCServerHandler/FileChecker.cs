using System;
using System.IO;
using System.Threading;

namespace MCServerHandler
{
    /// <summary>
    /// Checksfor the alive file every 10 seconds.
    /// </summary>
    internal class FileChecker
    {
        private const string AliveFileName = "server-running.file";
        /// <summary>
        /// Directory of the alive file
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// Loop runs till this is true.
        /// </summary>
        private bool CheckFile { get; set; }
        /// <summary>
        /// Check file thread.
        /// </summary>
        private Thread CheckThread { get; set; }

        /// <summary>
        /// Initializes the File checker.
        /// </summary>
        /// <param name="directory">directory to put the alive file.</param>
        public FileChecker(string directory)
        {
            if (!directory.EndsWith("/") && !directory.EndsWith("\\"))
                directory += "\\";

            Directory = directory;
        }

        /// <summary>
        /// Start the alive file checker on a new thread.
        /// </summary>
        public void Start()
        {
            CreateAliveFile();
            CheckThread = new Thread(CheckFileConstantly);
            CheckThread.Start();
        }

        /// <summary>
        /// Stops the alive checker thread.
        /// </summary>
        public void Stop()
        {
            CheckFile = false;
            File.Delete(Directory + AliveFileName);
        }

        /// <summary>
        /// Checks for the alive file, and invokes the FileDeleted event if it was deleted.
        /// </summary>
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

        /// <summary>
        /// Creates the alive file if it doesn't exist.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Checks if the file exists.
        /// </summary>
        /// <returns></returns>
        public bool CheckFileExists() => File.Exists(Directory + AliveFileName);

        /// <summary>
        /// Fires when the alive file is deleted.
        /// </summary>
        public event EventHandler<object> FileDeleted;
    }
}
