using Loggers;
using System.Diagnostics;
using Application.Minecraft.Util;

namespace Application.Minecraft.MinecraftServers.Utils
{
    /// <summary>
    /// Class for handling low level process events handling of a minecraft server.
    /// </summary>
    internal class MinecraftServerProcess
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
            // server.jar path
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
            // get the server.jar fileinfo
            FileInfo info = new(_serverFileName);
            var workingDir = info.DirectoryName;
            var simpleFileName = info.Name;

            // processstartinfo of cmd in the server folder
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

            // create cmd process
            _serverHandlerProcess = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };

            LogService.GetService<MinecraftLogger>().Log("server-process", $"Starting server {simpleFileName} with max-ram {_maxRam}.");

            // start cmd process and make it run the server handler
            _serverHandlerProcess.Start();
            _serverHandlerProcess.StandardInput.WriteLine($"\"{_serverHandlerPath}\" {simpleFileName} \"{_javaLocation}\" \"{workingDir}\" {_maxRam} {_initRam} & exit");
            _serverHandlerProcess.BeginErrorReadLine();
            _serverHandlerProcess.BeginOutputReadLine();


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


        public long GetStorage()
        {
            var info = new FileInfo(_serverFileName);
            long dirSize = FileHelper.DirSize(info.Directory);

            return dirSize;
        }
    }
}
