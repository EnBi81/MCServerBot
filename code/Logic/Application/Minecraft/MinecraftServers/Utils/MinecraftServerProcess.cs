using Application.Minecraft.Configs;
using Application.Minecraft.Util;
using Application.Minecraft.Versions;
using Loggers;
using System.Diagnostics;

namespace Application.Minecraft.MinecraftServers.Utils
{
    /// <summary>
    /// Class for handling low level process events handling of a minecraft server.
    /// </summary>
    internal class MinecraftServerProcess
    {
        private Process? _serverHandlerProcess;
        private readonly string _serverDirectory;
        private readonly string _javaLocation;
        private readonly string _serverHandlerPath;
        private readonly MinecraftServerConfig _serverConfig; 


        public MinecraftServerProcess(
            string serverDirectory,
            string javaLocation,
            string serverHandlerPath,
            MinecraftServerConfig config)
        {
            _serverDirectory = serverDirectory;
            _javaLocation = javaLocation;
            _serverHandlerPath = serverHandlerPath;
            _serverConfig = config;
        }

        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        public bool IsRunning => _serverHandlerProcess?.HasExited != true;

        /// <summary>
        /// Writes the text to the process' standard input.
        /// </summary>
        /// <param name="text">Text to write to the standard input.</param>
        public async Task WriteToStandardInputAsync(string text)
        {
            if (_serverHandlerProcess != null)
                await _serverHandlerProcess.StandardInput.WriteLineAsync(text);
        }

        /// <summary>
        /// Start the minecraft server process, and subscribe to all the process events.
        /// </summary>
        public async Task<Process> Start(IMinecraftVersion version)
        {
            if (!version.IsDownloaded)
                await version.DownloadAsync();
            
            var serverFilePath = version.AbsoluteJarPath;

            // processstartinfo of cmd in the server folder
            var processStartInfo = new ProcessStartInfo
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = false,
                //Arguments = $"\"{serverHandlerPath}\" {simpleFileName} \"{_javaLocation}\" \"{workingDir}\" {maxRam} {initRam}",
                FileName = "cmd.exe",
                WorkingDirectory = _serverDirectory
            };

            // create cmd process
            _serverHandlerProcess = new Process()
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
            };

            LogService.GetService<MinecraftLogger>().Log("server-process", $"Starting server in {_serverDirectory} with max-ram {_serverConfig.ServerMaxRamMB}.");

            // start cmd process and make it run the server handler
            _serverHandlerProcess.Start();
            _serverHandlerProcess.StandardInput.WriteLine($"\"{_serverHandlerPath}\" {serverFilePath} \"{_javaLocation}\" \"{_serverDirectory}\" {_serverConfig.ServerMaxRamMB} {_serverConfig.ServerInitRamMB} & exit");
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

            return _serverHandlerProcess;
        }

        public event EventHandler<string>? ErrorDataReceived;
        public event EventHandler<string>? OutputDataReceived;
        public event EventHandler? Exited;
        public event EventHandler<int>? ProcessIdReceived;


        public long GetStorage()
        {
            var info = new DirectoryInfo(_serverDirectory);
            long dirSize = FileHelper.DirSize(info);

            return dirSize;
        }
    }
}
