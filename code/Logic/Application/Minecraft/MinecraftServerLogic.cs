using Application.Minecraft.Configs;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.MinecraftServers.Utils;
using Application.Minecraft.States;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.Util;
using Application.Minecraft.Versions;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Model;
using System.Diagnostics;

namespace Application.Minecraft
{
    /// <summary>
    /// A representation of a single minecraft server.
    /// </summary>
    internal class MinecraftServerLogic : IMinecraftServer
    {

        /// <inheritdoc/>
        public long Id { get; }

        private string _serverName = null!;

        /// <inheritdoc/>
        public string ServerName
        {
            get => _serverName;
            set
            {
                _serverName = value;
                RaiseEvent(NameChanged, value);
                McServerInfos.Save(this);
            }
        }

        private IServerState _serverState;

        /// <inheritdoc/>
        public ServerStatus StatusCode => _serverState.Status;

        /// <inheritdoc/>
        public bool IsRunning => _serverState.IsRunning;


        /// <inheritdoc/>
        public DateTime? OnlineFrom { get; internal set; }

        /// <inheritdoc/>
        public IMinecraftServerProperties Properties { get; }

        /// <inheritdoc/>
        public int Port => int.Parse(Properties["server-port"] ?? "-1");

        /// <inheritdoc/>
        public string StorageSpace => FileHelper.StorageFormatter(StorageBytes);

        /// <inheritdoc/>
        public long StorageBytes
        {
            get => _storageBytes;
            internal set
            {
                _storageBytes = value;
                RaiseEvent(StorageMeasured, value);
            }
        }
        private long _storageBytes;

        private readonly Queue<ILogMessage> _logs = new ();
        /// <inheritdoc/>
        public ICollection<ILogMessage> Logs => _logs.ToList();

        /// <inheritdoc/>
        public Dictionary<string, IMinecraftPlayer> Players { get; } = new Dictionary<string, IMinecraftPlayer>();

        /// <inheritdoc/>
        public IMinecraftVersion MCVersion
        {
            get => _mcVersion;
            set
            {
                int versionCompared = Version.Parse(_mcVersion.Version).CompareTo(Version.Parse(value.Version));
                if (versionCompared > 0)
                    throw new MinecraftServerException($"Cannot downgrade from {_mcVersion.Version} to {value.Version}");
                else if (versionCompared == 0)
                    throw new MinecraftServerException($"Server is already on version {value.Version}");

                SetServerState<MaintenanceState>();
                
                _mcVersion = value;
                RaiseEvent(VersionChanged, value);
                McServerInfos.Save(this);
            }
        }
        private IMinecraftVersion _mcVersion;




        internal ProcessPerformanceReporter? PerformanceReporter { get; private set; }
        internal MinecraftServerProcess McServerProcess { get; }
        internal MinecraftServerInfos McServerInfos { get; }
        internal MinecraftServersFileHandler McServerFileHandler { get; }
        internal string ServerPath { get; }



        public MinecraftServerLogic(string serverFolderName, MinecraftConfig config, IMinecraftVersionCollection vsCollection) : this(0, serverFolderName, config)
        {
            McServerInfos.Load();

            var version = vsCollection[McServerInfos.Version];
            if (version is null)
                throw new MCInternalException($"Version {McServerInfos.Version} not found in version collection");


            Id = McServerInfos.Id;
            _serverName = McServerInfos.Name!;
            _mcVersion = version;

            SetServerState<OfflineState>();
        }


        public MinecraftServerLogic(long id, string serverName, 
            string serverFolderName, MinecraftConfig config, IMinecraftVersion version, 
            MinecraftServerCreationPropertiesDto? creationProperties) : this(id, serverFolderName, config)
        {
            _serverName = serverName;
            _mcVersion = version;

            McServerInfos.Save(this);
            
            Thread t = new Thread(() => SetServerState<MaintenanceState>(creationProperties));
            t.Start();
        }


        private MinecraftServerLogic(long id, string serverFolderName, MinecraftConfig config)
        {
            string serverPropertiesFileName = serverFolderName + "\\server.properties";
            string serverInfoFile = serverFolderName + "\\server.info";

            ServerPath = serverFolderName;
            Id = id;
            Properties = MinecraftServerProperties.GetProperties(serverPropertiesFileName);
            McServerInfos = new MinecraftServerInfos(serverInfoFile);
            McServerFileHandler = new MinecraftServersFileHandler(ServerPath);


            McServerProcess = new MinecraftServerProcess(
                serverDirectory: serverFolderName,
                javaLocation: config.JavaLocation,
                serverHandlerPath: config.MinecraftServerHandlerPath,
                maxRam: config.MinecraftServerMaxRamMB,
                initRam: config.MinecraftServerInitRamMB);

            SubscribeToProcessEvents();

            _mcVersion = null!;
            _serverState = null!;
            SetServerState<OfflineState>();

            PlayerLeft = null!;
            LogReceived = null!;
            NameChanged = null!;
            PlayerJoined = null!;
            StatusChange = null!;
            StorageMeasured = null!;
            PerformanceMeasured = null!;
            VersionChanged = null!;
        }


        /// <summary>
        /// Subscribes to the events fired by the <see cref="McServerProcess"/>
        /// </summary>
        private void SubscribeToProcessEvents()
        {
            McServerProcess.ErrorDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e, LogMessageType.Error_Message);
                HandleLog(logMess);
            };
            McServerProcess.OutputDataReceived += (s, e) =>
            {
                var logMess = new LogMessage(e, LogMessageType.System_Message);
                HandleLog(logMess);
            };
            McServerProcess.Exited += async (s, e) => await SetServerStateAsync<BackupAutoState>();
            McServerProcess.ProcessIdReceived += (s, processId) =>
            {
                PerformanceReporter = new ProcessPerformanceReporter(processId);
                PerformanceReporter.Start();
                PerformanceReporter.PerformanceMeasured += (s, data) =>
                {
                    (double cpu, long memory) = data;
                    RaiseEvent(PerformanceMeasured, (cpu, memory));
                };
            };
        }

        /// <inheritdoc/>
        public Task Start(UserEventData data) =>
            SetServerStateAsync<StartingState>(data.Username);


        /// <inheritdoc/>
        public Task WriteCommand(string? command, UserEventData data) =>
            _serverState.WriteCommand(command, data.Username);

        /// <inheritdoc/>
        public Task Shutdown(UserEventData data) =>
            SetServerStateAsync<ShuttingDownState>(data.Username);

        /// <summary>
        /// Handles the received log message according to the state of the server.
        /// </summary>
        /// <param name="logMessage">log message object representing the log message</param>
        protected void HandleLog(LogMessage logMessage) =>
            _serverState.HandleLog(logMessage);



        /// <inheritdoc/>
        public event EventHandler<ServerStatus> StatusChange;
        /// <inheritdoc/>
        public event EventHandler<ILogMessage> LogReceived;
        /// <inheritdoc/>
        public event EventHandler<IMinecraftPlayer> PlayerJoined;
        /// <inheritdoc/>
        public event EventHandler<IMinecraftPlayer> PlayerLeft;
        /// <inheritdoc/>
        public event EventHandler<(double CPU, long Memory)> PerformanceMeasured;
        /// <inheritdoc/>
        public event EventHandler<string> NameChanged;
        /// <inheritdoc/>
        public event EventHandler<IMinecraftVersion> VersionChanged;
        /// <inheritdoc/>
        public event EventHandler<long> StorageMeasured;


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
        internal void SetServerState<T>(params object?[] args) where T : IServerState =>
             SetServerStateAsync<T>(args).Wait();

        /// <summary>
        /// Change the state of the server. Please don't put abstract or interface classes, thank you.
        /// </summary>
        /// <typeparam name="T">New state, which implements the IServerState interface</typeparam>
        /// <exception cref="Exception">If the T is abstract or is an interface, or if it does not contain a public constructor which accepts a single MinecraftServer argument.</exception>
        internal async Task SetServerStateAsync<T>(params object?[] args) where T : IServerState
        {
            var type = typeof(T);
            
            // here we check that the type is neither abstract nor interface, and it has a constructor which takes a minecraft server.
            if (type.IsAbstract ||
               type.IsInterface ||
               type.GetConstructor(new Type[] { typeof(MinecraftServerLogic), typeof(string[]) }) == null) 
               //last line: check if T contains a public constructor which takes a MinecraftServer object and string array
            {
                throw new MCInternalException("Invalid State " + type.FullName);
            }


            var parameters = new object[] { this, args };

            object? nullableState = Activator.CreateInstance(type, parameters);
            if (nullableState is not IServerState state)
                throw new MCInternalException($"Error creating {type.FullName}");

            
            if (_serverState is IServerState stateTemp)
                if (!stateTemp.IsAllowedNextState(state))
                    return;

            _serverState = state;
            
            RaiseEvent(StatusChange, _serverState.Status);
            await state.Apply();
        }

        /// <summary>
        /// Set a player as online. Also if the player is not registered, it automatically registers it.
        /// </summary>
        /// <param name="username">Username of the player.</param>
        internal void SetPlayerOnline(string username)
        {
            if (!Players.ContainsKey(username))
                Players.Add(username, new MinecraftPlayer(username));

            ((MinecraftPlayer)Players[username]).SetOnline();
            RaiseEvent(PlayerJoined, Players[username]);
        }

        /// <summary>
        /// Set player as offline.
        /// </summary>
        /// <param name="username">Username of the player.</param>
        internal void SetPlayerOffline(string username)
        {
            Players.TryGetValue(username, out IMinecraftPlayer? player);

            if (player == null)
                return;

            ((MinecraftPlayer)player).SetOffline();
            RaiseEvent(PlayerLeft, player);
        }



        /// <summary>
        /// Adds a log to the Logs list, and raises a log received event.
        /// </summary>
        /// <param name="logMessage"></param>
        internal void AddLog(LogMessage logMessage)
        {
            if(_logs.Count >= 50)
                _ = _logs.Dequeue();
            _logs.Enqueue(logMessage);
            
            RaiseEvent(LogReceived, logMessage);
        }

        /// <summary>
        /// Starts the server process.
        /// </summary>
        /// <returns></returns>
        internal Task<Process> StartServerProcess() =>
            McServerProcess.Start(MCVersion);




        public static bool operator ==(MinecraftServerLogic? s1, MinecraftServerLogic? s2)
            => s1?.Equals(s2) ?? s2?.Equals(s1) ?? true;

        public static bool operator !=(MinecraftServerLogic? s1, MinecraftServerLogic? s2)
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

            if (obj is not MinecraftServerLogic server)
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
