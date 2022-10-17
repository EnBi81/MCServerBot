using Loggers;
using Application.Minecraft.Util;
using Application.Minecraft.Enums;
using Application.Minecraft.States;

namespace Application.Minecraft.MinecraftServers
{
    /// <summary>
    /// A representation of a single minecraft server.
    /// </summary>
    internal class MinecraftServer : IMinecraftServer
    {

        /// <summary>
        /// Id number, unique for a minecraft server.
        /// </summary>
        public int Id { get; } = 0;

        private string _serverName = null!;
        /// <summary>
        /// Gets or sets the name of the server. Raises a <see cref="NameChanged"/> event.
        /// </summary>
        public string ServerName
        {
            get => _serverName;
            set
            {
                if (value is null || value.Length < IMinecraftServer.NAME_MIN_LENGTH || value.Length > IMinecraftServer.NAME_MAX_LENGTH)
                    throw new ArgumentException($"Name length must be between {IMinecraftServer.NAME_MIN_LENGTH} and {IMinecraftServer.NAME_MAX_LENGTH}");

                _serverName = value;
                RaiseEvent(NameChanged, value);
            }
        }

        private IServerState _serverState;

        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        public ServerStatus Status => _serverState.Status;

        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        public bool IsRunning => _serverState.IsRunning;


        /// <summary>
        /// The time of the server when it became online, or null if the server is offline
        /// </summary>
        public DateTime? OnlineFrom { get; internal set; }

        /// <summary>
        /// Access to the properties file of the server.
        /// </summary>
        public MinecraftServerProperties Properties { get; }

        /// <summary>
        /// Gets the port associated with the server.
        /// </summary>
        public int Port => int.Parse(Properties["server-port"]);

        /// <summary>
        /// Phisical storage space on the disk of the server.
        /// </summary>
        public string StorageSpace => FileHelper.StorageFormatter(StorageBytes);

        /// <summary>
        /// Phisical storage space on the disk of the server in BYTES.
        /// </summary>
        public long StorageBytes { get; internal set; }

        /// <summary>
        /// All of the log messages the server or the users wrote.
        /// </summary>
        public List<LogMessage> Logs { get; } = new List<LogMessage>();

        /// <summary>
        /// Holding all the players who have played in the server, from the beginning of the current runtime.
        /// </summary>
        public Dictionary<string, MinecraftPlayer> Players { get; } = new Dictionary<string, MinecraftPlayer>();

        /// <summary>
        /// Performance reporter class, which measures the cpu and memory usage of the running minecraft server.
        /// </summary>
        internal ProcessPerformanceReporter? PerformanceReporter { get; private set; }

        /// <summary>
        /// Low level process handling of the minecraft server.
        /// </summary>
        internal MinecraftServerProcess McServerProcess { get; }



        /// <summary>
        /// Initializes the MinecraftServer object
        /// </summary>
        /// <param name="serverName">Name of the server</param>
        /// <param name="serverFolderName">Folder path of the server</param>
        public MinecraftServer(string serverName, string serverFolderName)
        {
            string serverFileName = serverFolderName + "\\server.jar";
            string serverPropertiesFileName = serverFolderName + "\\server.properties";

            ServerName = serverName;
            Properties = MinecraftServerProperties.GetProperties(serverPropertiesFileName);

            var config = MinecraftConfig.Instance;
            if (config == null)
                throw new Exception("MinecraftConfig instance is not created.");

            McServerProcess = new MinecraftServerProcess(
                serverFileName: serverFileName,
                javaLocation: config.JavaLocation,
                serverHandlerPath: config.MinecraftServerHandlerPath,
                maxRam: config.MinecraftServerMaxRamMB,
                initRam: config.MinecraftServerInitRamMB);

            SubscribeToProcessEvents();

            _serverState = null!;
            SetServerState<OfflineState>();
            LogService.GetService<MinecraftLogger>().Log("server", $"Server {ServerName} created");

            StatusChange = null!;
            LogReceived = null!;
            PlayerJoined = null!;
            PlayerLeft = null!;
            PerformanceMeasured = null!;
            NameChanged = null!;
        }

        /// <summary>
        /// Subscribes to the events fired by the <see cref="McServerProcess"/>
        /// </summary>
        private void SubscribeToProcessEvents()
        {
            McServerProcess.ErrorDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e, LogMessage.LogMessageType.Error_Message);
                HandleLog(logMess);
            };
            McServerProcess.OutputDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e, LogMessage.LogMessageType.System_Message);
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

        /// <summary>
        /// Starts the server
        /// </summary>
        /// <param name="user">username of the user who initiates the start of the server.</param>
        public void Start(string user = "Admin") =>
            _serverState.Start(user);

        /// <summary>
        /// Writes a command to the minecraft serves based on the state of the server.
        /// </summary>
        /// <param name="command">command to send to the minecraft server.</param>
        /// <param name="user">username of the user who sends the command.</param>
        public void WriteCommand(string command, string user = "Admin") =>
            _serverState.WriteCommand(command, user);

        /// <summary>
        /// Shuts down the minecraft server if it is online.
        /// </summary>
        /// <param name="user">username of the user who initiates the start of the server.</param>
        public void Shutdown(string user = "Admin") =>
            _serverState.Stop(user);

        /// <summary>
        /// Handles the received log message according to the state of the server.
        /// </summary>
        /// <param name="logMessage">log message object representing the log message</param>
        protected void HandleLog(LogMessage logMessage) =>
            _serverState.HandleLog(logMessage);



        /// <summary>
        /// Fired when the server has changed status.
        /// </summary>
        public event EventHandler<ServerStatus> StatusChange;
        /// <summary>
        /// Fired when a log message received.
        /// </summary>
        public event EventHandler<LogMessage> LogReceived;
        /// <summary>
        /// Fired when a player has joined to the server.
        /// </summary>
        public event EventHandler<MinecraftPlayer> PlayerJoined;
        /// <summary>
        /// Fired when a player has left the server.
        /// </summary>
        public event EventHandler<MinecraftPlayer> PlayerLeft;
        /// <summary>
        /// Fired when performance has been measured of the minecraft server process.
        /// </summary>
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;
        /// <summary>
        /// Fired when the server has changed its name.
        /// </summary>
        public event EventHandler<string> NameChanged;


        /// <summary>
        /// Raises an event of the eventhandler put in the evt parameter.
        /// </summary>
        /// <typeparam name="T">Type of the event argument of the event handler.</typeparam>
        /// <param name="evt">Event handler to invoke.</param>
        /// <param name="param">Event data to invoke the handler with.</param>
        protected void RaiseEvent<T>(EventHandler<T> evt, T param)
            => evt?.Invoke(this, param);


        /// <summary>
        /// Change the state of the server. Please don't put abstract or interface classes, thank you.
        /// </summary>
        /// <typeparam name="T">New state, which implements the IServerState interface</typeparam>
        /// <exception cref="Exception">If the T is abstract or is an interface, or if it does not contain a public constructor which accepts a single MinecraftServer argument.</exception>
        internal void SetServerState<T>() where T : IServerState
        {
            var type = typeof(T);

            // here we check that the type is neither abstract nor interface, and it has a constructor which takes a minecraft server.
            if (type.IsAbstract ||
               type.IsInterface ||
               type.GetConstructor(new Type[] { typeof(MinecraftServer) }) == null) //last line: check if T contains a public constructor which takes a MinecraftServer object
            {
                throw new Exception("Invalid State " + type.FullName);
            }


            var parameters = new object[] { this };

            object? nullableState = Activator.CreateInstance(type, parameters);
            if (nullableState is not IServerState state)
                throw new Exception($"Error creating {type.FullName}");

            _serverState = state;
            LogService.GetService<MinecraftLogger>().Log("server", $"Status Change: {_serverState.Status}");
            RaiseEvent(StatusChange, _serverState.Status);
        }

        /// <summary>
        /// Set a player as online. Also if the player is not registered, it automatically registers it.
        /// </summary>
        /// <param name="username">Username of the player.</param>
        internal void SetPlayerOnline(string username)
        {
            if (!Players.ContainsKey(username))
                Players.Add(username, new MinecraftPlayer(username));

            Players[username].SetOnline();
            RaiseEvent(PlayerJoined, Players[username]);
        }

        /// <summary>
        /// Set player as offline.
        /// </summary>
        /// <param name="username">Username of the player.</param>
        internal void SetPlayerOffline(string username)
        {
            Players.TryGetValue(username, out MinecraftPlayer? player);

            if (player == null)
                return;

            player.SetOffline();
            RaiseEvent(PlayerLeft, player);
        }



        /// <summary>
        /// Adds a log to the Logs list, and raises a log received event.
        /// </summary>
        /// <param name="logMessage"></param>
        internal void AddLog(LogMessage logMessage)
        {
            LogService.GetService<MinecraftLogger>().Log("server", $"{logMessage.MessageType} received: {logMessage.Message}", ConsoleColor.DarkGreen);
            Logs.Add(logMessage);
            RaiseEvent(LogReceived, logMessage);
        }




        public static bool operator ==(MinecraftServer? s1, MinecraftServer? s2)
            => s1?.Equals(s2) ?? s2?.Equals(s1) ?? true;

        public static bool operator !=(MinecraftServer? s1, MinecraftServer? s2)
            => !(s1 == s2);

        /// <summary>
        /// Checks if the object given in the argument is equal to the current object.
        /// </summary>
        /// <param name="obj">Object to check.</param>
        /// <returns>True if the object is a MinecraftServer and has the same name as the current server, else false.</returns>
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            if (obj is not MinecraftServer server)
                return false;

            return ServerName == server.ServerName;
        }

        /// <summary>
        /// Gets the hash code of the current object.
        /// </summary>
        /// <returns>The hash code of the current object.</returns>
        public override int GetHashCode()
        {
            unchecked // disable overflow, for the unlikely possibility that you
            {         // are compiling with overflow-checking enabled
                int hash = 27;
                hash = 13 * hash + ServerName.GetHashCode();
                return hash;
            }
        }
    }
}
