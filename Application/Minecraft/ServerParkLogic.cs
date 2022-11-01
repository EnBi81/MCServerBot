using Application.DAOs;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.Util;
using Loggers;
using Shared.DTOs;
using Shared.EventHandlers;
using Shared.Exceptions;
using Shared.Model;
using System.Collections.ObjectModel;

namespace Application.Minecraft
{
    /// <summary>
    /// Main logic of the ServerPark.
    /// </summary>
    internal class ServerParkLogic : IServerPark
    {
        /// <summary>
        /// Path of the folder the minecraft servers are stored
        /// </summary>
        internal string ServersFolder { get; }
        /// <summary>
        /// Path of the folder which contains the previously deleted servers
        /// </summary>
        internal string DeletedServersFolder { get; } 
        /// <summary>
        /// Path of an empty server folder (this is copied into the <see cref="ServersFolder"/> when a new server is created)
        /// </summary>
        internal string EmptyServersFolder { get; } 


        private long _serverIdCounter;
        private readonly IDatabaseAccess _databaseAccess;
        private readonly MinecraftConfig _config;


        internal ServerParkLogic(IDatabaseAccess dataAccess, MinecraftConfig config)
        {
            _databaseAccess = dataAccess;
            _config = config;

            ServersFolder = _config.MinecraftServersBaseFolder + "Servers\\";
            DeletedServersFolder = _config.MinecraftServersBaseFolder + "Deleted Servers\\";
            EmptyServersFolder = _config.MinecraftServersBaseFolder + "Empty Server\\";


            ActiveServerChange = null!;
            ActiveServerPlayerLeft = null!;
            ActiveServerPlayerJoined = null!;
            ActiveServerLogReceived = null!;
            ActiveServerPerformanceMeasured = null!;
            ActiveServerStatusChange = null!;

            ServerAdded = null!;
            ServerDeleted = null!;
            ServerNameChanged = null!;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            _serverIdCounter = await _databaseAccess.ServerParkDataAccess.GetMaxServerId();

            DirectoryInfo info = new(ServersFolder);
            var serverFolders = info.GetDirectories();

            foreach (var serverFolder in serverFolders)  // loop through all the directories and create the server instances
            {
                string folderName = serverFolder.Name;

                if (!ulong.TryParse(folderName, out var _))
                {
                    LogService.GetService<MinecraftLogger>().Log("serverpark", $"ERROR: cannot convert folder {folderName} to ulong.", ConsoleColor.Red);
                    continue;
                }

                var mcServer = new MinecraftServer(_databaseAccess.MinecraftDataAccess, serverFolder.FullName, _config);
                RegisterMcServer(mcServer);
            }
        }


        /// <inheritdoc/>
        public IMinecraftServer? ActiveServer { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<long, IMinecraftServer> MCServers => new ReadOnlyDictionary<long, IMinecraftServer>(ServerCollection);




        /// <summary>
        /// List of all minecraft server instances
        /// </summary>
        internal Dictionary<long, IMinecraftServer> ServerCollection { get; } = new();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IMinecraftServer GetServer(long id)
        {
            ThrowIfServerNotExists(id);
            return ServerCollection[id];
        }


        /// <inheritdoc/>
        private void SetActiveServer(long id)
        {
            if (ActiveServer != null)
            {
                if (ActiveServer.IsRunning)
                    throw new ServerParkException("Another Server is Running Already!");
            }

            IMinecraftServer server = GetServer(id);

            if (ActiveServer == server)
                return;

            UnSubscribeEventTrackers(ActiveServer);
            ActiveServer = server;
            SubscribeEventTrackers(ActiveServer);


            InvokeActiveServerChanged(ActiveServer);
        }


        /// <inheritdoc/>
        public Task StartServer(long id, UserEventData user)
        {
            ValidateMaxStorage();

            SetActiveServer(id);
            ActiveServer?.Start(user.Username);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopActiveServer(UserEventData user)
        {
            if (ActiveServer == null || !ActiveServer.IsRunning)
                throw new ServerParkException("Server is not running!");

            ActiveServer?.Shutdown(user.Username);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task ToggleServer(long id, UserEventData user) =>
            ActiveServer?.IsRunning ?? false 
            ? StopActiveServer(user) 
            : StartServer(id, user);



        /// <inheritdoc/>
        public Task<IMinecraftServer> CreateServer(string serverName, UserEventData user)
        {
            CreateServerCheck(serverName);

            long newServerId = _serverIdCounter++;

            string destDir = ServersFolder + newServerId;
            Directory.CreateDirectory(destDir);
            FileHelper.CopyDirectory(EmptyServersFolder, destDir);

            var mcServer = new MinecraftServer(_databaseAccess.MinecraftDataAccess, newServerId, serverName, destDir, _config);
            RegisterMcServer(mcServer);

            return Task.FromResult((IMinecraftServer)mcServer);
        }


        /// <inheritdoc/>
        public Task<IMinecraftServer> RenameServer(long id, string newName, UserEventData user)
        {
            ValidateNameLength(newName);

            ThrowIfServerNotExists(id);
            ThrowIfServerRunning(id);


            if (ServerNameExist(newName))
                throw new ServerParkException($"The name '{newName}' is already taken");



            var server = ServerCollection[id];
            var oldName = server.ServerName;
            server.ServerName = newName;

            InvokeServerNameChange(oldName, newName);

            return Task.FromResult(server);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> DeleteServer(long id, UserEventData user)
        {
            ThrowIfServerNotExists(id);
            ThrowIfServerRunning(id);

            string newDir = $"{DeletedServersFolder}{id}-{DateTime.Now:yyyy-MM-dd HH-mm-ss}";
            FileHelper.MoveDirectory(ServersFolder + id, newDir);

            ServerCollection.Remove(id, out IMinecraftServer? server);

            if (server != null)
                InvokeServerDeleted(server);

            return Task.FromResult(server!);
        }

        /// <summary>
        /// Check if the name already exists.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>true if it exists, else false.</returns>
        private bool ServerNameExist(string name) => ServerCollection.Values.Any(server => server.ServerName == name);

        /// <summary>
        /// Checks if the name's length is valid, throws exception if yes.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <exception cref="Exception">if the name is not valid.</exception>
        private static void ValidateNameLength(string name)
        {
            if (!(name.Length <= IMinecraftServer.NAME_MAX_LENGTH && name.Length >= IMinecraftServer.NAME_MIN_LENGTH))
                throw new ServerParkException($"Name must be no longer than {IMinecraftServer.NAME_MAX_LENGTH} characters and more than {IMinecraftServer.NAME_MIN_LENGTH}!");
        }



        private void ValidateMaxStorage()
        {
            var dir = _config.MinecraftServersBaseFolder;
            long maxDiskSpaceByte = (long)_config.MaxSumOfDiskSpaceGB * (1024 * 1024 * 1024);

            (bool overflow, long measured) = FileHelper.CheckStorageOverflow(dir, maxDiskSpaceByte);

            string measuredString = FileHelper.StorageFormatter(measured);

            LogService.GetService<MinecraftLogger>().Log("serverpark", "Storage measured: " + measuredString);

            if (overflow)
            {
                LogService.GetService<MinecraftLogger>().Log("serverpark", "Storage OVERFLOW", ConsoleColor.Red);
                throw new ServerParkException($"Disk space full. Max disk space allocated: {_config.MaxSumOfDiskSpaceGB} GB." +
                    $" Current storage: {measuredString}.");
            }
        }


        /// <summary>
        /// Register a new minecraft server object to the program.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="folderPath"></param>
        private void RegisterMcServer(MinecraftServer server)
        {
            ServerCollection.Add(server.Id, server);
            InvokeServerAdded(server);
        }


        /// <summary>
        /// Checks if a server with the specific name can be created.
        /// </summary>
        /// <param name="serverName"></param>
        /// <exception cref="Exception">If the server cannot be created.</exception>
        private void CreateServerCheck(string serverName)
        {
            ValidateNameLength(serverName);

            if (ServerNameExist(serverName))
                throw new ServerParkException($"The name {serverName} is already taken");

            ValidateMaxStorage();
        }

        private bool ServerExists(long id) => ServerCollection.ContainsKey(id); 

        private void ThrowIfServerRunning(long id)
        {
            if (ActiveServer != null && ActiveServer.Id == id && ActiveServer.IsRunning)
                throw new ServerParkException($"To delete {ActiveServer.ServerName} server, first make sure it is stopped.");
        }

        private void ThrowIfServerNotExists(long id)
        {
            if (!ServerExists(id))
                throw new ServerParkException($"The server '{id}' does not exist.");
        }



        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<ILogMessage>> ActiveServerLogReceived;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerJoined;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerLeft;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<(string CPU, string Memory)>> ActiveServerPerformanceMeasured;

        /// <inheritdoc/>
        public event EventHandler<ValueChangedEventArgs<string>> ServerNameChanged;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted;




        /// <summary>
        /// Subscribe ServerPark events on a specific minecraft server.
        /// </summary>
        /// <param name="server">server to subscribe</param>
        private void SubscribeEventTrackers(IMinecraftServer server)
        {
            server.StatusChange += InvokeStatusTracker;
            server.LogReceived += InvokeLogReceived;
            server.PlayerLeft += InvokePlayerLeft;
            server.PlayerJoined += InvokePlayerJoined;
            server.PerformanceMeasured += InvokePerformanceMeasured;
        }

        /// <summary>
        /// Unsubscribe ServerPark events from a minecraft server.
        /// </summary>
        /// <param name="server">server to unsubscribe from</param>
        private void UnSubscribeEventTrackers(IMinecraftServer? server)
        {
            if (server == null)
                return;

            server.StatusChange -= InvokeStatusTracker;
            server.LogReceived -= InvokeLogReceived;
            server.PlayerLeft -= InvokePlayerLeft;
            server.PlayerJoined -= InvokePlayerJoined;
            server.PerformanceMeasured -= InvokePerformanceMeasured;
        }


        // IMinecraft events
        private void InvokePerformanceMeasured(object? sender, (string CPU, string Memory) e) =>
            ActiveServerPerformanceMeasured?.Invoke(sender, new(e));
        private void InvokePlayerJoined(object? sender, IMinecraftPlayer e) =>
            ActiveServerPlayerJoined?.Invoke(sender, new(e));
        private void InvokePlayerLeft(object? sender, IMinecraftPlayer e) =>
            ActiveServerPlayerLeft?.Invoke(sender, new(e));
        private void InvokeLogReceived(object? sender, ILogMessage e) =>
            ActiveServerLogReceived?.Invoke(sender, new(e));
        private void InvokeStatusTracker(object? sender, ServerStatus e) =>
            ActiveServerStatusChange?.Invoke(sender, new(e));


        // ServerPark events
        private void InvokeActiveServerChanged(IMinecraftServer activeServer) =>
            ActiveServerChange?.Invoke(this, new(activeServer));
        private void InvokeServerNameChange(string oldName, string newName) =>
            ServerNameChanged?.Invoke(this, new(oldName, newName));
        private void InvokeServerAdded(IMinecraftServer addedServer) =>
            ServerAdded?.Invoke(this, new(addedServer));
        private void InvokeServerDeleted(IMinecraftServer deletedServer) =>
            ServerDeleted?.Invoke(this, new(deletedServer));

    }
}
