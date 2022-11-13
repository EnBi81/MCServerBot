using Application.DAOs;
using Application.Minecraft.Util;
using Loggers;
using Shared.DTOs;
using Shared.EventHandlers;
using Shared.Exceptions;
using Shared.Model;

namespace Application.Minecraft
{
    /// <summary>
    /// Checks and validates if a method is allowed to use. If not, an exception is thrown.
    /// </summary>
    internal class ServerParkInputValidation : IServerPark
    {
        private readonly IServerPark _serverParkLogic;
        private readonly MinecraftConfig _config;

        public ServerParkInputValidation(IDatabaseAccess dataAccess, MinecraftConfig config, MinecraftLogger logger)
        {
            _serverParkLogic = new ServerParkLogic(dataAccess, config, logger);
            _config = config;
        }


        /// <inheritdoc/>
        public Task InitializeAsync() => _serverParkLogic.InitializeAsync();

        /// <inheritdoc/>
        public IMinecraftServer? ActiveServer => _serverParkLogic.ActiveServer;

        /// <inheritdoc/>
        public IReadOnlyDictionary<long, IMinecraftServer> MCServers => _serverParkLogic.MCServers;

        /// <inheritdoc/>
        public IMinecraftVersionCollection MinecraftVersionCollection => _serverParkLogic.MinecraftVersionCollection;



        /// <inheritdoc/>
        public IMinecraftServer GetServer(long id)
        {
            ThrowIfServerNotExists(id);
            return _serverParkLogic.GetServer(id);
        }

        /// <inheritdoc/>
        public Task StartServer(long id, UserEventData user = default)
        {
            ValidateMaxStorage();
            ThrowIfServerNotExists(id);
            if (ActiveServer != null)
            {
                if (ActiveServer.IsRunning)
                    throw new ServerParkException("Another Server is Running Already!");
            }

            return _serverParkLogic.StartServer(id, user);
        }

        /// <inheritdoc/>
        public Task StopActiveServer(UserEventData user = default)
        {
            if (ActiveServer == null || !ActiveServer.IsRunning)
                throw new ServerParkException("Server is not running!");

            return _serverParkLogic.StopActiveServer(user);
        }

        /// <inheritdoc/>
        public Task ToggleServer(long id, UserEventData user = default) =>
            _serverParkLogic.ToggleServer(id, user);

        /// <inheritdoc/>
        public Task<IMinecraftServer> CreateServer(ServerChangeableDto dto, UserEventData user = default)
        {
            var name = dto.NewName;
            CreateServerCheck(ref name);

            if (dto.Version is not null)
                CheckVersionExist(dto.Version);

            return _serverParkLogic.CreateServer(dto, user);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> ModifyServer(long id, ServerChangeableDto dto, UserEventData user = default)
        {
            if (dto.NewName is null && dto.Version is null)
                throw new MinecraftServerArgumentException("Invalid modify: no data to change");

            ThrowIfServerNotExists(id);
            ThrowIfServerRunning(id);
            
            var newName = dto.NewName;
            if(newName is not null)
            {
                ValidateNameLength(ref newName!);
                if (ServerNameExist(newName))
                    throw new ServerParkException($"The name '{newName}' is already taken");
            }

            var newVersion = dto.Version;
            if(newVersion is not null)
            {
                CheckVersionExist(newVersion);
            }

            return _serverParkLogic.ModifyServer(id, dto, user);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> DeleteServer(long id, UserEventData user = default)
        {
            ThrowIfServerNotExists(id);
            ThrowIfServerRunning(id);

            return _serverParkLogic.DeleteServer(id, user);
        }

        private void CheckVersionExist(string? version)
        {
            if (MinecraftVersionCollection[version] is null)
                throw new ServerParkException($"The version '{version}' does not exist");
        }


        /// <summary>
        /// Check if the name already exists.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>true if it exists, else false.</returns>
        private bool ServerNameExist(string name) => MCServers.Values.Any(server => server.ServerName == name);

        /// <summary>
        /// Checks if the name's length is valid, throws exception if yes.
        /// </summary>
        /// <param name="name">name to check</param>
        /// <exception cref="Exception">if the name is not valid.</exception>
        private static void ValidateNameLength(ref string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new MinecraftServerArgumentException("server name must not be null or white space");

            name = name.Trim();

            if (name.Length is not (<= IMinecraftServer.NAME_MAX_LENGTH and >= IMinecraftServer.NAME_MIN_LENGTH))
                throw new MinecraftServerArgumentException($"Name must be no longer than {IMinecraftServer.NAME_MAX_LENGTH} characters and more than {IMinecraftServer.NAME_MIN_LENGTH}!");
        }



        private void ValidateMaxStorage()
        {
            var dir = _config.MinecraftServersBaseFolder;
            long maxDiskSpaceByte = (long)_config.MaxSumOfDiskSpaceGB * (1024 * 1024 * 1024);

            (bool overflow, long measured) = FileHelper.CheckStorageOverflow(dir, maxDiskSpaceByte);

            string measuredString = FileHelper.StorageFormatter(measured);

            if (overflow)
            {
                throw new ServerParkException($"Disk space full. Max disk space allocated: {_config.MaxSumOfDiskSpaceGB} GB." +
                    $" Current storage: {measuredString}.");
            }
        }


        /// <summary>
        /// Checks if a server with the specific name can be created.
        /// </summary>
        /// <param name="serverName"></param>
        /// <exception cref="Exception">If the server cannot be created.</exception>
        private void CreateServerCheck(ref string? serverName)
        {
            ValidateNameLength(ref serverName);

            if (ServerNameExist(serverName))
                throw new ServerParkException($"The name {serverName} is already taken");

            ValidateMaxStorage();
        }

        private bool ServerExists(long id) => MCServers.ContainsKey(id);

        private void ThrowIfServerRunning(long id)
        {
            if (ActiveServer != null && ActiveServer.Id == id && ActiveServer.IsRunning)
                throw new ServerParkException($"To delete {ActiveServer.ServerName} server, first make sure it is stopped.");
        }

        /// <summary>
        /// Throws ServerParkException if the server id is not registered.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="ServerParkException"></exception>
        private void ThrowIfServerNotExists(long id)
        {
            if (!ServerExists(id))
                throw new ServerParkException($"The server '{id}' does not exist.");
        }


        public event EventHandler<ValueEventArgs<IMinecraftServer>> ActiveServerChange
        {
            add => _serverParkLogic.ActiveServerChange += value; 
            remove => _serverParkLogic.ActiveServerChange -= value;
        }

        public event EventHandler<ServerValueEventArgs<ServerStatus>> ActiveServerStatusChange
        {
            add => _serverParkLogic.ActiveServerStatusChange += value;
            remove => _serverParkLogic.ActiveServerStatusChange -= value;
        }

        public event EventHandler<ServerValueEventArgs<ILogMessage>> ActiveServerLogReceived
        {
            add => _serverParkLogic.ActiveServerLogReceived += value;
            remove => _serverParkLogic.ActiveServerLogReceived -= value;
        }

        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerJoined
        {
            add => _serverParkLogic.ActiveServerPlayerJoined += value;
            remove => _serverParkLogic.ActiveServerPlayerJoined -= value;
        }

        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerLeft
        {
            add => _serverParkLogic.ActiveServerPlayerLeft += value;
            remove => _serverParkLogic.ActiveServerPlayerLeft -= value;
        }

        public event EventHandler<ServerValueEventArgs<(double CPU, long Memory)>> ActiveServerPerformanceMeasured
        {
            add => _serverParkLogic.ActiveServerPerformanceMeasured += value;
            remove => _serverParkLogic.ActiveServerPerformanceMeasured -= value;
        }

        public event EventHandler<ServerValueEventArgs<ServerChangeableDto>> ServerModified
        {
            add => _serverParkLogic.ServerModified += value;
            remove => _serverParkLogic.ServerModified -= value;
        }

        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded
        {
            add => _serverParkLogic.ServerAdded += value;
            remove => _serverParkLogic.ServerAdded -= value;
        }

        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted
        {
            add => _serverParkLogic.ServerDeleted += value;
            remove => _serverParkLogic.ServerDeleted -= value;
        }
    }
}
