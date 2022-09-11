using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Management;
using MCWebServer.Log;
using MCWebServer.MinecraftServer.Util;
using MCWebServer.MinecraftServer.Enums;
using MCWebServer.Config;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// A representation of a single minecraft server.
    /// </summary>
    public class MinecraftServer // TODO: Do state machine on status
    {
        public const int NAME_MIN_LENGTH = 4;
        public const int NAME_MAX_LENGTH = 35;


        private string _serverName;
        public string ServerName { get => _serverName;
            set
            {
                if (value is null || value.Length < NAME_MIN_LENGTH || value.Length > NAME_MAX_LENGTH)
                    throw new ArgumentException("Name length must be between 4 and 35");

                _serverName = value;
            } }

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
                        StorageSpace = _mcServerProcess.GetStorage();
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



        private ProcessPerformanceReporter _performanceReporter;
        private MinecraftServerProcess _mcServerProcess;


        public MinecraftServer(string serverName, string serverFolderName)
        {
            string serverFileName = serverFolderName + "\\server.jar";
            string serverPropertiesFileName = serverFolderName + "\\server.properties";

            ServerName = serverName;
            Properties = MinecraftServerProperties.GetProperties(serverPropertiesFileName);

            

            Config.Config config = Config.Config.Instance;

            _mcServerProcess = new MinecraftServerProcess(
                serverFileName:     serverFileName,
                javaLocation:       config.JavaLocation,
                serverHandlerPath:  config.MinecraftServerHandlerPath,
                maxRam:             config.MinecraftServerMaxRamMB,
                initRam:            config.MinecraftServerInitRamMB);

            SubscribeToProcessEvents();


            Status = ServerStatus.Offline;
            LogService.GetService<MinecraftLogger>().Log("server", $"Server {ServerName} created");
        }

        private void SubscribeToProcessEvents()
        {
            _mcServerProcess.ErrorDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e.Data, LogMessage.LogMessageType.Error_Message);
                AddLog(logMess);
            };

            _mcServerProcess.OutputDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e.Data, LogMessage.LogMessageType.System_Message);
                AddLog(logMess);
            };
            _mcServerProcess.Exited += (s, e) =>
            {
                Status = ServerStatus.Offline;
                _performanceReporter.Stop();
            };

            _mcServerProcess.ProcessIdReceived += (s, e) =>
            {
                _performanceReporter = new ProcessPerformanceReporter(e);
                _performanceReporter.Start();
            };
        }

        public void Start(string user = "Admin")
        {
            if (IsRunning())
                throw new Exception("Server is already running");

            Status = ServerStatus.Starting;
            var logMessage = new LogMessage(user + ": " + "Starting Server", LogMessage.LogMessageType.User_Message);
            AddLog(logMessage);

            

            

            

            _mcServerProcess.Start();
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
                _mcServerProcess.WriteToStandardInput(command);
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


        
    }
}
