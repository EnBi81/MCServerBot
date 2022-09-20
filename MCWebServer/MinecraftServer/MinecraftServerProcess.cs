using MCWebServer.Log;
using System.Diagnostics;
using System.IO;
using System;
using MCWebServer.MinecraftServer.Util;
using System.Threading.Tasks;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// Class for handling low level process events handling of a minecraft server.
    /// </summary>
    public class MinecraftServerProcess
    {
        private Process? _serverHandlerProcess;
        private readonly string _serverFileName;
        private readonly string _javaLocation;
        private readonly string _serverHandlerPath;
        private readonly int _maxRam;
        private readonly int _initRam;


        public MinecraftServerProcess(
            string serverFileName, 
            string javaLocation, 
            string serverHandlerPath,
            int maxRam, 
            int initRam)
        {
            _serverFileName = serverFileName;
            _javaLocation = javaLocation;
            _serverHandlerPath = serverHandlerPath;
            _maxRam = maxRam;
            _initRam = initRam;
        }

        /// <summary>
        /// Writes the text to the process' standard input.
        /// </summary>
        /// <param name="text">Text to write to the standard input.</param>
        public void WriteToStandardInput(string text)
        {
            _serverHandlerProcess?.StandardInput.WriteLine(text);
        }

        /// <summary>
        /// Start the minecraft server process, and subscribe to all the process events.
        /// </summary>
        public void Start()
        {
            FileInfo info = new FileInfo(_serverFileName);
            var workingDir = info.DirectoryName;
            var simpleFileName = info.Name;

            var processStartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = false,
                //Arguments = $"\"{serverHandlerPath}\" {simpleFileName} \"{_javaLocation}\" \"{workingDir}\" {maxRam} {initRam}",
                FileName = "cmd.exe",
                WorkingDirectory = workingDir
            };

            _serverHandlerProcess = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };

            LogService.GetService<MinecraftLogger>().Log("server-process", $"Starting server {simpleFileName} with max-ram {_maxRam}.");

            _serverHandlerProcess.Start();
            _serverHandlerProcess.StandardInput.WriteLine($"\"{_serverHandlerPath}\" {simpleFileName} \"{_javaLocation}\" \"{workingDir}\" {_maxRam} {_initRam} & exit");
            _serverHandlerProcess.BeginErrorReadLine();
            _serverHandlerProcess.BeginOutputReadLine();

            LogService.GetService<MinecraftLogger>().Log("server-process", $"Starting server {simpleFileName} with max-ram {_maxRam}.");

            _serverHandlerProcess.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    ErrorDataReceived?.Invoke(this, e.Data);
                }
            };

            int messageCount = 0;
            _serverHandlerProcess.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null)
                    return;

                if (messageCount < 5) //ignore the first 4 message
                {
                    if (++messageCount == 5) // the fifth line is the processId of the server
                    {
                        int minecraftProcessId = int.Parse(e.Data);
                        ProcessIdReceived?.Invoke(this, minecraftProcessId);
                    }

                    return;
                }

                OutputDataReceived?.Invoke(this, e.Data);
            };
            _serverHandlerProcess.Exited += (s, e) =>
            {
                _serverHandlerProcess = null;
                Exited?.Invoke(this, e);
            };
        }

        public event EventHandler<string>? ErrorDataReceived;
        public event EventHandler<string>? OutputDataReceived;
        public event EventHandler? Exited;
        public event EventHandler<int>? ProcessIdReceived;


        public string GetStorage()
        {
            var info = new FileInfo(_serverFileName);
            double size = FileHelper.DirSize(info.Directory);

            string storage = FileHelper.StorageFormatter(size);
            LogService.GetService<MinecraftLogger>().Log("server", $"Storage measured: " + storage);

            return storage;
        }
    }
}
