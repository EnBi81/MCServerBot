using System.Collections.Generic;
using MCWebServer.Log;
using MCWebServer.MinecraftServer.Util;
using MCWebServer.MinecraftServer.Enums;
using MCWebServer.MinecraftServer.States;

namespace MCWebServer.MinecraftServer
{
    /// <summary>
    /// A representation of a single minecraft server.
    /// </summary>
    internal class MinecraftServer : IMinecraftServer // TODO: Do state machine on status
    {
        


        private string _serverName;
        public string ServerName
        {
            get => _serverName; set
            {
                if (value is null || value.Length < IMinecraftServer.NAME_MIN_LENGTH || value.Length > IMinecraftServer.NAME_MAX_LENGTH)
                    throw new ArgumentException($"Name length must be between {IMinecraftServer.NAME_MIN_LENGTH} and {IMinecraftServer.NAME_MAX_LENGTH}");

                _serverName = value;
            } 
        }

        public List<LogMessage> Logs { get; } = new List<LogMessage>();

        private IBaseState _serverState;
        public ServerStatus Status => _serverState.Status;
        public bool IsRunning => _serverState.IsRunning;

        public DateTime? OnlineFrom { get; internal set; }
        public MinecraftServerProperties Properties { get; }


        public Dictionary<string, MinecraftPlayer> Players { get; } = new Dictionary<string, MinecraftPlayer>();


        public string StorageSpace { get; internal set; }



        internal ProcessPerformanceReporter PerformanceReporter { get; private set; }
        internal MinecraftServerProcess McServerProcess { get; }


        public MinecraftServer(string serverName, string serverFolderName)
        {
            string serverFileName = serverFolderName + "\\server.jar";
            string serverPropertiesFileName = serverFolderName + "\\server.properties";

            ServerName = serverName;
            Properties = MinecraftServerProperties.GetProperties(serverPropertiesFileName);

            

            Config.Config config = Config.Config.Instance;

            McServerProcess = new MinecraftServerProcess(
                serverFileName:     serverFileName,
                javaLocation:       config.JavaLocation,
                serverHandlerPath:  config.MinecraftServerHandlerPath,
                maxRam:             config.MinecraftServerMaxRamMB,
                initRam:            config.MinecraftServerInitRamMB);

            SubscribeToProcessEvents();


            SetServerState<OfflineState>();
            LogService.GetService<MinecraftLogger>().Log("server", $"Server {ServerName} created");
        }


        private void SubscribeToProcessEvents()
        {
            McServerProcess.ErrorDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e.Data, LogMessage.LogMessageType.Error_Message);
                HandleLog(logMess);
            };

            McServerProcess.OutputDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e.Data, LogMessage.LogMessageType.System_Message);
                HandleLog(logMess);
            };
            McServerProcess.Exited += (s, e) => SetServerState<OfflineState>();

            McServerProcess.ProcessIdReceived += (s, processId) =>
            {
                PerformanceReporter = new ProcessPerformanceReporter(processId);
                PerformanceReporter.Start();
                PerformanceReporter.PerformanceMeasured += (s, data) => RaiseEvent(PerformanceMeasured, data);
            };
        }

        public void Start(string user = "Admin") =>
            _serverState.Start(user);

        public void WriteCommand(string command, string user = "Admin") =>
            _serverState.WriteCommand(command, user);

        public void Shutdown(string user = "Admin") => 
            _serverState.Stop(user);

        internal void HandleLog(LogMessage logMessage) =>
            _serverState.HandleLog(logMessage);



        public event EventHandler<ServerStatus> StatusChange;
        public event EventHandler<LogMessage> LogReceived;
        public event EventHandler<MinecraftPlayer> PlayerJoined;
        public event EventHandler<MinecraftPlayer> PlayerLeft;
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;


        internal void RaiseEvent<T>(EventHandler<T> evt, T param) 
            => evt?.Invoke(this, param);


        internal void SetServerState<T>() where T : IBaseState
        {
            var type = typeof(T);

            // here we check that the type is neither abstract nor interface, and it has a constructor which takes a minecraft server.
            if(type.IsAbstract ||      
               type.IsInterface ||
               type.GetConstructor(new Type[] {typeof(MinecraftServer)}) == null)
            {
                throw new Exception("Invalid State " + type.FullName);
            }


            var parameters = new object[] { this };
            _serverState = (IBaseState) Activator.CreateInstance(type, parameters);
            LogService.GetService<MinecraftLogger>().Log("server", $"Status Change: {_serverState.Status}");
            RaiseEvent(StatusChange, _serverState.Status);
        }

        internal void SetPlayerOnline(string username)
        {
            if (!Players.ContainsKey(username))
                Players.Add(username, new MinecraftPlayer(username));

            Players[username].SetOnline();
            RaiseEvent(PlayerJoined, Players[username]);
        }

        internal void SetPlayerOffline(string username)
        {
            Players.TryGetValue(username, out MinecraftPlayer player);
            player.SetOffline();
            RaiseEvent(PlayerLeft, player);
        }

        internal void AddLog(LogMessage logMessage)
        {
            LogService.GetService<MinecraftLogger>().Log("server", $"{logMessage.MessageType} received: {logMessage.Message}", ConsoleColor.DarkGreen);
            Logs.Add(logMessage);
            RaiseEvent(LogReceived, logMessage);
        }

        


        public static bool operator ==(MinecraftServer s1, MinecraftServer s2)
            => s1?.Equals(s2) ?? s2?.Equals(s1) ?? true;

        public static bool operator !=(MinecraftServer s1, MinecraftServer s2)
            => !(s1 == s2);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (obj is not MinecraftServer server)
                return false;

            return ServerName == server.ServerName;
        }
    }
}
