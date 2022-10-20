using Application.Minecraft.Enums;
using Application.Minecraft.EventHandlers;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.Util;
using DataStorage.DataObjects;
using Loggers;
using System.Collections.ObjectModel;

namespace Application.Minecraft
{
    internal class ServerPark : IServerPark
    {
        /// <summary>
        /// Path of the folder the minecraft servers are stored
        /// </summary>
        internal static string ServersFolder { get; } = MinecraftConfig.Instance.MinecraftServersBaseFolder + "Servers\\";
        /// <summary>
        /// Path of the folder which contains the previously deleted servers
        /// </summary>
        internal static string DeletedServersFolder { get; } = MinecraftConfig.Instance.MinecraftServersBaseFolder + "Deleted Servers\\";
        /// <summary>
        /// Path of an empty server folder (this is copied into the <see cref="ServersFolder"/> when a new server is created)
        /// </summary>
        internal static string EmptyServersFolder { get; } = MinecraftConfig.Instance.MinecraftServersBaseFolder + "Empty Server\\";


        private ulong _serverId;


        internal ServerPark(ulong serverId)
        {
            _serverId = serverId;

            ServerCollection.Clear();

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

        internal ServerPark InitializeAsync()
        {
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

                var mcServer = new MinecraftServer(serverFolder.FullName);
                RegisterMcServer(mcServer);
            }

            return this;
        }


        /// <inheritdoc/>
        public IMinecraftServer? ActiveServer { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, IMinecraftServer> MCServers => new ReadOnlyDictionary<string, IMinecraftServer>(ServerCollection);




        /// <summary>
        /// List of all minecraft server instances
        /// </summary>
        internal Dictionary<string, IMinecraftServer> ServerCollection { get; } = new();




        /// <inheritdoc/>
        private void SetActiveServer(string serverName)
        {
            if (ActiveServer != null)
            {
                if (ActiveServer.IsRunning)
                    throw new Exception("Another Server is Running Already!");
            }

            if (!MCServers.TryGetValue(serverName, out IMinecraftServer? server))
            {
                throw new Exception("Server not found!");
            }

            if (ActiveServer == server)
                return;

            UnSubscribeEventTrackers(ActiveServer);
            ActiveServer = server;
            SubscribeEventTrackers(ActiveServer);


            InvokeActiveServerChanged(ActiveServer);
        }


        /// <inheritdoc/>
        public Task StartServer(string serverName, UserEventData user)
        {
            ValidateMaxStorage();

            SetActiveServer(serverName);
            ActiveServer?.Start(user.Username);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopActiveServer(UserEventData user)
        {
            if (ActiveServer == null || !ActiveServer.IsRunning)
                throw new Exception("Server is not running!");

            ActiveServer?.Shutdown(user.Username);

            return Task.CompletedTask;
        }


        /// <inheritdoc/>
        public Task ToggleServer(string serverName, UserEventData user) =>
            ActiveServer?.IsRunning ?? false 
            ? StopActiveServer(user) 
            : StartServer(serverName, user);



        /// <inheritdoc/>
        public Task<IMinecraftServer> CreateServer(string serverName, UserEventData user)
        {
            CreateServerCheck(serverName);

            ulong newServerId = _serverId++;

            string destDir = ServersFolder + newServerId;
            Directory.CreateDirectory(destDir);
            FileHelper.CopyDirectory(EmptyServersFolder, destDir);

            var mcServer = new MinecraftServer(newServerId, serverName, destDir);
            RegisterMcServer(mcServer);

            return Task.FromResult((IMinecraftServer)mcServer);
        }


        /// <inheritdoc/>
        public Task<IMinecraftServer> RenameServer(string oldName, string newName, UserEventData user)
        {
            ValidateNameLength(newName);

            if (!ServerNameExist(oldName))
                throw new Exception($"The server '{oldName}' does not exist.");

            if (ServerNameExist(newName))
                throw new Exception($"The name '{newName}' is already taken");

            if (ActiveServer != null && ActiveServer.ServerName == oldName && ActiveServer.IsRunning)
                throw new Exception($"{oldName} is Runnning! Please stop the server first.");

            FileHelper.MoveDirectory(ServersFolder + oldName, ServersFolder + newName);

            ServerCollection.Remove(oldName, out IMinecraftServer? server);

            IMinecraftServer notNullServer = server!;

            ServerCollection.Add(newName, notNullServer);
            notNullServer.ServerName = newName;

            InvokeServerNameChange(oldName, newName);

            return Task.FromResult(notNullServer);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> DeleteServer(string name, UserEventData user)
        {
            if (!ServerNameExist(name))
                throw new Exception($"The server '{name}' does not exist.");

            if (ActiveServer != null && ActiveServer.ServerName == name && ActiveServer.IsRunning)
                throw new Exception($"To delete this server, first make sure it is stopped.");

            string newDir = DeletedServersFolder + name + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            FileHelper.MoveDirectory(ServersFolder + name, newDir);

            ServerCollection.Remove(name, out IMinecraftServer? server);

            if (server != null)
                InvokeServerDeleted(server);

            return Task.FromResult(server!);
        }

        /// <summary>
        /// Check if the name already exists.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>true if it exists, else false.</returns>
        private bool ServerNameExist(string name) => MCServers.ContainsKey(name);

        /// <summary>
        /// Checks if the name's length is valid, throws exception if yes.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <exception cref="Exception">if the name is not valid.</exception>
        private static void ValidateNameLength(string name)
        {
            if (!(name.Length <= IMinecraftServer.NAME_MAX_LENGTH && name.Length >= IMinecraftServer.NAME_MIN_LENGTH))
                throw new Exception($"Name must be no longer than {IMinecraftServer.NAME_MAX_LENGTH} characters and more than {IMinecraftServer.NAME_MIN_LENGTH}!");
        }



        private static void ValidateMaxStorage()
        {
            var dir = MinecraftConfig.Instance.MinecraftServersBaseFolder;
            long maxDiskSpaceByte = (long)MinecraftConfig.Instance.MaxSumOfDiskSpaceGB * (1024 * 1024 * 1024);

            (bool overflow, long measured) = FileHelper.CheckStorageOverflow(dir, maxDiskSpaceByte);

            string measuredString = FileHelper.StorageFormatter(measured);

            LogService.GetService<MinecraftLogger>().Log("serverpark", "Storage measured: " + measuredString);

            if (overflow)
            {
                LogService.GetService<MinecraftLogger>().Log("serverpark", "Storage OVERFLOW", ConsoleColor.Red);
                throw new Exception($"Disk space full. Max disk space allocated: {MinecraftConfig.Instance.MaxSumOfDiskSpaceGB} GB." +
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
            ServerCollection.Add(server.ServerName, server);
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
                throw new Exception($"The name {serverName} is already taken");

            ValidateMaxStorage();
        }



        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<ServerStatus>> ActiveServerStatusChange;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<LogMessage>> ActiveServerLogReceived;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerJoined;

        /// <inheritdoc/>
        public event EventHandler<ValueEventArgs<MinecraftPlayer>> ActiveServerPlayerLeft;

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
        private void InvokePlayerJoined(object? sender, MinecraftPlayer e) =>
            ActiveServerPlayerJoined?.Invoke(sender, new(e));
        private void InvokePlayerLeft(object? sender, MinecraftPlayer e) =>
            ActiveServerPlayerLeft?.Invoke(sender, new(e));
        private void InvokeLogReceived(object? sender, LogMessage e) =>
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
