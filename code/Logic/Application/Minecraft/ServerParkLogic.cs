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
    /// ServerParkInputValidation already checks the important stuff, so here we need to execute the actions.
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
        private readonly MinecraftLogger _logger;


        internal ServerParkLogic(IDatabaseAccess dataAccess, MinecraftConfig config, MinecraftLogger logger)
        {
            _databaseAccess = dataAccess;
            _config = config;
            _logger = logger;

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
                    throw new MCInternalException($"ERROR: cannot convert folder {folderName} to ulong. Please remove that folder from the Servers directory!");
                }

                var mcServer = new MinecraftServer(_databaseAccess.MinecraftDataAccess, _logger, serverFolder.FullName, _config);
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


        /// <inheritdoc/>
        public IMinecraftServer GetServer(long id) =>
            ServerCollection[id];



        /// <inheritdoc/>
        private void SetActiveServer(long id)
        {
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
            SetActiveServer(id);
            ActiveServer?.Start(user);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopActiveServer(UserEventData user)
        {
            ActiveServer?.Shutdown(user);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task ToggleServer(long id, UserEventData user) =>
            ActiveServer?.IsRunning ?? false
            ? StopActiveServer(user)
            : StartServer(id, user);



        /// <inheritdoc/>
        public Task<IMinecraftServer> CreateServer(string? serverName, UserEventData user)
        {
            // synchronization increment
            long newServerId = Interlocked.Increment(ref _serverIdCounter);

            string destDir = ServersFolder + newServerId;
            Directory.CreateDirectory(destDir);
            FileHelper.CopyDirectory(EmptyServersFolder, destDir);

            var mcServer = new MinecraftServer(_databaseAccess.MinecraftDataAccess, _logger, newServerId, serverName!, destDir, _config);
            RegisterMcServer(mcServer);

            return Task.FromResult((IMinecraftServer)mcServer);
        }


        /// <inheritdoc/>
        public Task<IMinecraftServer> RenameServer(long id, string? newName, UserEventData user)
        {
            string serverName = newName!;

            var server = ServerCollection[id];
            var oldName = server.ServerName;
            server.ServerName = serverName;

            InvokeServerNameChange(server, oldName, serverName);

            return Task.FromResult(server);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> DeleteServer(long id, UserEventData user)
        {
            string newDir = $"{DeletedServersFolder}{id}-{DateTime.Now:yyyy-MM-dd HH-mm-ss}";
            FileHelper.MoveDirectory(ServersFolder + id, newDir);

            ServerCollection.Remove(id, out IMinecraftServer? server);

            if (server != null)
                InvokeServerDeleted(server);

            return Task.FromResult(server!);
        }

        /// <summary>
        /// Register a new minecraft server object to the program.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="folderPath"></param>
        private void RegisterMcServer(IMinecraftServer server)
        {
            ServerCollection.Add(server.Id, server);
            InvokeServerAdded(server);
        }



        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange;

        /// <inheritdoc/>
        public event EventHandler<ServerValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <inheritdoc/>
        public event EventHandler<ServerValueEventArgs<ILogMessage>> ActiveServerLogReceived;

        /// <inheritdoc/>
        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerJoined;

        /// <inheritdoc/>
        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerLeft;

        /// <inheritdoc/>
        public event EventHandler<ServerValueEventArgs<(double CPU, long Memory)>> ActiveServerPerformanceMeasured;

        /// <inheritdoc/>
        public event EventHandler<ServerValueChangedEventArgs<string>> ServerNameChanged;

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
        private void InvokePerformanceMeasured(object? sender, (double CPU, long Memory) e) =>
            ActiveServerPerformanceMeasured?.Invoke(sender, new(e, (IMinecraftServer)sender!));
        private void InvokePlayerJoined(object? sender, IMinecraftPlayer e) =>
            ActiveServerPlayerJoined?.Invoke(sender, new(e, (IMinecraftServer)sender!));
        private void InvokePlayerLeft(object? sender, IMinecraftPlayer e) =>
            ActiveServerPlayerLeft?.Invoke(sender, new(e, (IMinecraftServer)sender!));
        private void InvokeLogReceived(object? sender, ILogMessage e) =>
            ActiveServerLogReceived?.Invoke(sender, new(e, (IMinecraftServer)sender!));
        private void InvokeStatusTracker(object? sender, ServerStatus e) =>
            ActiveServerStatusChange?.Invoke(sender, new(e, (IMinecraftServer)sender!));


        // ServerPark events
        private void InvokeActiveServerChanged(IMinecraftServer activeServer) =>
            ActiveServerChange?.Invoke(this, new(activeServer));
        private void InvokeServerNameChange(IMinecraftServer server, string oldName, string newName) =>
            ServerNameChanged?.Invoke(this, new(oldName, newName, server));
        private void InvokeServerAdded(IMinecraftServer addedServer) =>
            ServerAdded?.Invoke(this, new(addedServer));
        private void InvokeServerDeleted(IMinecraftServer deletedServer) =>
            ServerDeleted?.Invoke(this, new(deletedServer));

    }
}
