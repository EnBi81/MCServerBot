using APIModel.DTOs;
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
        private readonly IServerPark _serverPark;
        private readonly MinecraftConfig _config;

        public ServerParkInputValidation(IDatabaseAccess dataAccess, MinecraftConfig config, MinecraftLogger logger)
        {
            _serverPark = new ServerParkLogic(dataAccess, config, logger);
            _config = config;
        }


        /// <inheritdoc/>
        public Task InitializeAsync() => _serverPark.InitializeAsync();

        /// <inheritdoc/>
        public IMinecraftServer? ActiveServer => _serverPark.ActiveServer;

        /// <inheritdoc/>
        public IReadOnlyDictionary<long, IMinecraftServer> MCServers => _serverPark.MCServers;

        /// <inheritdoc/>
        public IMinecraftVersionCollection MinecraftVersionCollection => _serverPark.MinecraftVersionCollection;



        /// <inheritdoc/>
        public IMinecraftServer GetServer(long id)
        {
            ThrowIfServerNotExists(id);
            return _serverPark.GetServer(id);
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

            return _serverPark.StartServer(id, user);
        }

        /// <inheritdoc/>
        public Task StopActiveServer(UserEventData user = default)
        {
            if (ActiveServer == null || !ActiveServer.IsRunning)
                throw new ServerParkException("Server is not running!");

            return _serverPark.StopActiveServer(user);
        }

        /// <inheritdoc/>
        public Task ToggleServer(long id, UserEventData user = default) =>
            _serverPark.ToggleServer(id, user);

        /// <inheritdoc/>
        public Task<IMinecraftServer> CreateServer(ServerCreationDto dto, UserEventData user = default)
        {
            var name = dto.NewName;
            CreateServerCheck(ref name);

            if (dto.Version is not null)
                CheckVersionExist(dto.Version);
            
            dto.Properties ??= new MinecraftServerCreationPropertiesDto();
            dto.Properties.ValidateAndRetrieveData();

            return _serverPark.CreateServer(dto, user);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> ModifyServer(long id, ModifyServerDto dto, UserEventData user = default)
        {
            if (dto.GetType().GetProperties().All(prop => prop.GetValue(dto) == null))
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

            var newProps = dto.Properties;
            if (newProps is not null)
            {
                newProps.ValidateAndRetrieveData();
            }


            return _serverPark.ModifyServer(id, dto, user);
        }

        /// <inheritdoc/>
        public Task<IMinecraftServer> DeleteServer(long id, UserEventData user = default)
        {
            ThrowIfServerNotExists(id);
            ThrowIfServerRunning(id);

            return _serverPark.DeleteServer(id, user);
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
            var server = GetServer(id);

            if (server.IsRunning)
                throw new ServerParkException($"To delete {server.ServerName} server, first make sure it is stopped.");
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
        

        public event EventHandler<ServerValueEventArgs<ServerStatus>> ActiveServerStatusChange
        {
            add => _serverPark.ActiveServerStatusChange += value;
            remove => _serverPark.ActiveServerStatusChange -= value;
        }

        public event EventHandler<ServerValueEventArgs<ILogMessage>> ActiveServerLogReceived
        {
            add => _serverPark.ActiveServerLogReceived += value;
            remove => _serverPark.ActiveServerLogReceived -= value;
        }

        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerJoined
        {
            add => _serverPark.ActiveServerPlayerJoined += value;
            remove => _serverPark.ActiveServerPlayerJoined -= value;
        }

        public event EventHandler<ServerValueEventArgs<IMinecraftPlayer>> ActiveServerPlayerLeft
        {
            add => _serverPark.ActiveServerPlayerLeft += value;
            remove => _serverPark.ActiveServerPlayerLeft -= value;
        }

        public event EventHandler<ServerValueEventArgs<(double CPU, long Memory)>> ActiveServerPerformanceMeasured
        {
            add => _serverPark.ActiveServerPerformanceMeasured += value;
            remove => _serverPark.ActiveServerPerformanceMeasured -= value;
        }

        public event EventHandler<ServerValueEventArgs<ModifyServerDto>> ServerModified
        {
            add => _serverPark.ServerModified += value;
            remove => _serverPark.ServerModified -= value;
        }

        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded
        {
            add => _serverPark.ServerAdded += value;
            remove => _serverPark.ServerAdded -= value;
        }

        public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted
        {
            add => _serverPark.ServerDeleted += value;
            remove => _serverPark.ServerDeleted -= value;
        }
    }
}
