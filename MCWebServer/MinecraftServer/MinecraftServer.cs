using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Management;
using MCWebServer.Log;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// A representation of a single minecraft server.
    /// </summary>
    public class MinecraftServer
    {
        public string ServerName { get; }
        public int Port => int.Parse(Properties["server-port"]);
        public List<LogMessage> Logs { get; } = new List<LogMessage>();

        private ServerStatus _status = ServerStatus.Offline;
        public ServerStatus Status
        {
            get => _status;
            private set
            {
                _status = value;
                LogService.GetService<MinecraftLogger>().Log("server", $"Status Change: {value}");

                if (value == ServerStatus.Online)
                {
                    OnlineFrom = DateTime.Now;
                }
                else
                {
                    if(value == ServerStatus.ShuttingDown)
                    {
                        foreach (var player in OnlinePlayers)
                        {
                            Players[player.Username].SetOffline();
                            RaiseEvent(PlayerLeft, player);
                        }
                            
                    }
                    else if(value == ServerStatus.Offline)
                    {
                        StorageSpace = GetStorage(_serverFileName);
                    }
                    OnlineFrom = null;
                }


                RaiseEvent(StatusChange, _status);
            }
        }
        public DateTime? OnlineFrom { get; private set; }
        public MinecraftServerProperties Properties { get; }

        public List<MinecraftPlayer> OnlinePlayers
        {
            get
            {
                return (from player in Players.Values where player.OnlineFrom.HasValue select player).ToList();
            }
        }

        public Dictionary<string, MinecraftPlayer> Players { get; } = new Dictionary<string, MinecraftPlayer>();
        public string StorageSpace { get; private set; }

        private Process _serverHandlerProcess;
        private Process _minecraftProcess;
        private readonly string _serverFileName;
        private readonly string _javaLocation;


        private Thread _performanceThread = null;



        public MinecraftServer(string serverName, string serverFolderName, string javaLocation)
        {
            ServerName = serverName;
            _serverFileName = serverFolderName + "\\server.jar";
            Properties = MinecraftServerProperties.GetProperties(serverFolderName + "\\server.properties");
            _javaLocation = javaLocation;
            Status = ServerStatus.Offline;

            LogService.GetService<MinecraftLogger>().Log("server", $"Server {ServerName} created");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private void PerformanceReporter()
        {
            var mcProcess = _minecraftProcess;

            var objQuery = new ObjectQuery("select * from Win32_Process WHERE ProcessID = " + mcProcess.Id);
            var moSearcher = new ManagementObjectSearcher(objQuery);
            DateTime firstSample = DateTime.MinValue, secondSample;

            double OldProcessorUsage = 0;
            double ProcessorUsage = 20;
            double msPassed;
            ulong u_OldCPU = 0;
            string cpu = "";
            string memory = "";
            while (Status != ServerStatus.Offline)
            {
                var gets = moSearcher.Get();
                foreach (ManagementObject mObj in gets)
                {
                    try
                    {
                        if (firstSample == DateTime.MinValue)
                        {
                            firstSample = DateTime.Now;
                            mObj.Get();
                            u_OldCPU = (ulong)mObj["UserModeTime"] + (ulong)mObj["KernelModeTime"];
                        }
                        else
                        {
                            secondSample = DateTime.Now;
                            mObj.Get();
                            ulong u_newCPU = (ulong)mObj["UserModeTime"] + (ulong)mObj["KernelModeTime"];

                            msPassed = (secondSample - firstSample).TotalMilliseconds;
                            OldProcessorUsage = ProcessorUsage;
                            ProcessorUsage = (u_newCPU - u_OldCPU) / (msPassed * 100.0 * Environment.ProcessorCount);

                            u_OldCPU = u_newCPU;
                            firstSample = secondSample;
                        }


                        mcProcess.Refresh();
                        memory = (mcProcess.WorkingSet64 / (1024 * 1024)) + " MB";
                        cpu = ProcessorUsage.ToString("0.00") + "%";

                        RaiseEvent(PerformanceMeasured, (cpu, memory));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                        Console.WriteLine(ex.InnerException.Message);
                    }
                }

                int waitTime = ProcessorUsage > 10 ? 1 : 5;
                double cpuDifference = ProcessorUsage - OldProcessorUsage;

                if (Math.Abs(cpuDifference) < 6)
                    waitTime *= 2;


                LogService.GetService<MinecraftLogger>().Log("server", $"Performance measurement: cpu {cpu}, memory {memory}. " +
                    $"Next Measurement in {waitTime}");
                Thread.Sleep(waitTime * 1000);
            }

            
            moSearcher.Dispose();
            RaiseEvent(PerformanceMeasured, ("0%", "0 MB"));
        }


        public void Start(string user = "Admin")
        {
            if (IsRunning())
                throw new Exception("Server is already running");

            Status = ServerStatus.Starting;
            var logMessage = new LogMessage(user + ": " + "Starting Server", LogMessage.LogMessageType.User_Message);
            AddLog(logMessage);

            var serverHandlerPath = Config.Config.Instance.MinecraftServerHandlerPath;

            FileInfo info = new FileInfo(_serverFileName);
            var workingDir = info.DirectoryName;
            var simpleFileName = info.Name;
            var maxRam = Config.Config.Instance.MinecraftServerMaxRamMB;
            var initRam = Config.Config.Instance.MinecraftServerInitRamMB;

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

            LogService.GetService<MinecraftLogger>().Log("server", $"Starting server {simpleFileName} with max-ram {maxRam}.");

            _serverHandlerProcess.Start();
            _serverHandlerProcess.StandardInput.WriteLine($"\"{serverHandlerPath}\" {simpleFileName} \"{_javaLocation}\" \"{workingDir}\" {maxRam} {initRam} & exit");
            _serverHandlerProcess.BeginErrorReadLine();
            _serverHandlerProcess.BeginOutputReadLine();

            _serverHandlerProcess.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    var logMess = new LogMessage(e.Data, LogMessage.LogMessageType.Error_Message);
                    AddLog(logMess);
                }
            };

            int messageCount = 0;
            _serverHandlerProcess.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null)
                    return;

                if (messageCount < 5)
                {
                    if (++messageCount == 5)
                    {
                        int minecraftProcessId = int.Parse(e.Data);
                        _minecraftProcess = Process.GetProcessById(minecraftProcessId);

                        _performanceThread = new Thread(PerformanceReporter);
                        _performanceThread.Start();
                    }
                        
                    return;
                }

                var logMess = new LogMessage(e.Data, LogMessage.LogMessageType.System_Message);
                AddLog(logMess);
            };
            _serverHandlerProcess.Exited += (s, e) =>
            {
                Status = ServerStatus.Offline;
                _serverHandlerProcess = null;
            };
        }


        protected void AddLog(LogMessage logMessage)
        {
            var log = logMessage.Message;

            string baseTimeRegex = "\\[(\\d{2}:){2}\\d{2}\\] \\[Server thread\\/INFO\\]: ";
            Regex startupDoneRegex = new(baseTimeRegex + "Done \\([\\d.s]+\\)! For help, type \"help\"");
            Regex playerJoinedRegex = new(baseTimeRegex + "([a-zA-Z0-9_]+) joined the game");
            Regex playerLeftRegex = new(baseTimeRegex + "([a-zA-Z0-9_]+) left the game");
            Regex shutdownRegex = new(baseTimeRegex + "Stopping the server");

            LogService.GetService<MinecraftLogger>().Log("server", $"{logMessage.MessageType} received: {logMessage.Message}", ConsoleColor.DarkGreen);

            Logs.Add(logMessage);
            RaiseEvent(LogReceived, logMessage);

            try
            {
                // [14:02:39] [Server thread/INFO]: Done (44.552s)! For help, type "help"
                if (startupDoneRegex.IsMatch(log))
                    Status = ServerStatus.Online;

                // [21:34:35] [Server thread/INFO]: Enbi81 joined the game
                else if (playerJoinedRegex.IsMatch(log))
                {
                    var match = playerJoinedRegex.Match(log);
                    var cap = match.Groups[2];

                    var username = cap.Value;
                    if (!Players.ContainsKey(username))
                        Players.Add(username, new MinecraftPlayer(username));

                    Players[username].SetOnline();
                    RaiseEvent(PlayerJoined, Players[username]);
                }

                // [21:35:08] [Server thread/INFO]: Enbi81 left the game
                else if (playerLeftRegex.IsMatch(log))
                {
                    var match = playerLeftRegex.Match(log);
                    var cap = match.Groups[2];

                    Players[cap.Value].SetOffline();
                    RaiseEvent(PlayerLeft, Players[cap.Value]);
                }

                else if (shutdownRegex.IsMatch(log))
                {
                    Status = ServerStatus.ShuttingDown;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            
        }

        public void WriteCommand(string command, string user = "Admin")
        {
            try
            {
                _serverHandlerProcess.StandardInput.WriteLine(command);
                var logMess = new LogMessage(user + ": " + command, LogMessage.LogMessageType.User_Message);
                AddLog(logMess);
            }
            catch
            {
                throw new Exception("Server is not online!");
            }

        }

        public bool IsRunning()
        {
            return Status == ServerStatus.Online
                || Status == ServerStatus.Starting
                || Status == ServerStatus.ShuttingDown;
        }

        public void Shutdown(string user = "Admin")
        {
            LogService.GetService<MinecraftLogger>().Log("server", $"Shutdown request by: " + user);

            if (Status == ServerStatus.Offline)
                throw new Exception("Server is offline");
            if (Status == ServerStatus.ShuttingDown)
                throw new Exception("Server is already shutting down");
            if (Status == ServerStatus.Starting)
                throw new Exception("Server is Starting, please wait till it is online");


            try
            {
                WriteCommand("stop", user);
            }
            catch { }
        }



        public event EventHandler<ServerStatus> StatusChange;
        public event EventHandler<LogMessage> LogReceived;
        public event EventHandler<MinecraftPlayer> PlayerJoined;
        public event EventHandler<MinecraftPlayer> PlayerLeft;
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;

        protected void RaiseEvent<T>(EventHandler<T> evt, T param)
        {
            //Console.WriteLine($"Event raised: {evt.Method.Name} with data: {param}");
            evt?.Invoke(this, param);
        }

        
        public static bool operator ==(MinecraftServer s1, MinecraftServer s2)
        {
            if (s1 is null || s2 is null)
                return s1 is null && s2 is null;

            return s1.ServerName == s2.ServerName;
        }

        public static bool operator !=(MinecraftServer s1, MinecraftServer s2)
            => !(s1 == s2);


        private static string GetStorage(string fileName)
        {
            var info = new FileInfo(fileName);
            double size = DirSize(info.Directory);
            string measurement = "B";

            if (size > 1024)
            {
                size /= 1024;
                measurement = "KB";

                if (size > 1024)
                {
                    size /= 1024;
                    measurement = "MB";

                    if (size > 1024)
                    {
                        size /= 1024;
                        measurement = "GB";
                    }
                }
            }

            string storage = Math.Round(size, 2) + " " + measurement;
            LogService.GetService<MinecraftLogger>().Log("server", $"Storage measured: " + storage);

            return storage;
        }
        private static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
    }
}
