using Application.DAOs.Database;
using Loggers;
using Shared.DTOs;
using Shared.Model;

namespace Application.Minecraft.MinecraftServers
{
    /// <summary>
    /// Proxy class for logging and handling database records.
    /// </summary>
    internal class MinecraftServer : IMinecraftServer
    {
        private readonly MinecraftLogger _logger;
        private readonly IMinecraftDataAccess _eventRegister;
        private readonly MinecraftServerLogic _minecraftServerLogic;

        public MinecraftServer(IMinecraftDataAccess dataAccess, MinecraftLogger logger,
            string serverFolderName, MinecraftConfig config) : this(dataAccess, logger)
        {
            _minecraftServerLogic = InitLogicServer(() => new MinecraftServerLogic(serverFolderName, config));
            _logger.Log(_logger.MinecraftServer, $"Server {ServerName} created");
            Startup();
        }


        public MinecraftServer(IMinecraftDataAccess dataAccess, MinecraftLogger logger,
            long id, string serverName, string serverFolderName, MinecraftConfig config) : this(dataAccess, logger)
        {
            _minecraftServerLogic = InitLogicServer(() => new MinecraftServerLogic(id, serverName, serverFolderName, config));
            _logger.Log(_logger.MinecraftServer, $"Server {ServerName} created");
            Startup();
        }

        private MinecraftServer(IMinecraftDataAccess dataAccess, MinecraftLogger logger)
        {
            _eventRegister = dataAccess;
            _logger = logger;
            _minecraftServerLogic = null!;
        }

        private MinecraftServerLogic InitLogicServer(Func<MinecraftServerLogic> func)
        {
            try
            {
                return func();
            }
            catch (Exception e)
            {
                _logger.Error(_logger.MinecraftServer, e);
                throw;
            }
        }

        /// <summary>
        /// Starts up the listeners.
        /// </summary>
        private void Startup()
        {
            HandleDatabaseEvents();
            HandleLogging();
        }

        /// <summary>
        /// Listens to the minecraft server and adds the relevant events to the database.
        /// </summary>
        private void HandleDatabaseEvents()
        {
            _minecraftServerLogic.PerformanceMeasured += (s, e) =>
                _eventRegister.AddMeasurement(Id, e.CPU, e.Memory);

            _minecraftServerLogic.PlayerJoined += (s, e) =>
                _eventRegister.PlayerJoined(Id, e.Username);

            _minecraftServerLogic.PlayerLeft += (s, e) =>
                _eventRegister.PlayerLeft(Id, e.Username);

            _minecraftServerLogic.StorageMeasured += (s, e) =>
                _eventRegister.SetDiskSize(Id, e);
        }

        /// <summary>
        /// Handles the loggings.
        /// </summary>
        private void HandleLogging()
        {
            string mcServer = _logger.MinecraftServer;

            _minecraftServerLogic.LogReceived += (s, e)
                => _logger.Log(mcServer + "-log", e.Message);
            _minecraftServerLogic.PerformanceMeasured += (s, e)
                => _logger.Log(mcServer + "-performance", $"CPU: {e.CPU:0.00}%  Memory: {e.Memory / (1024 * 1024)} MB");
            _minecraftServerLogic.PlayerJoined += (s, e)
                => _logger.Log(mcServer + "-player", $"Player joined: " + e.Username);
            _minecraftServerLogic.PlayerLeft += (s, e)
                => _logger.Log(mcServer + "-player", $"Player left: " + e.Username);
            _minecraftServerLogic.StatusChange += (s, e)
                => _logger.Log(mcServer + "-status", $"New status: " + e.DisplayString());
            _minecraftServerLogic.StorageMeasured += (s, e)
                => _logger.Log(mcServer + "-storage", $"Storage measured: {e / (1024 * 1024)} MB");
        }

        /// <inheritdoc/>
        public long Id => _minecraftServerLogic.Id;

        /// <inheritdoc/>
        public string ServerName { get => _minecraftServerLogic.ServerName; set => _minecraftServerLogic.ServerName = value; }

        /// <inheritdoc/>
        public ServerStatus Status => _minecraftServerLogic.Status;

        /// <inheritdoc/>
        public bool IsRunning => _minecraftServerLogic.IsRunning;

        /// <inheritdoc/>
        public List<ILogMessage> Logs => _minecraftServerLogic.Logs;

        /// <inheritdoc/>
        public DateTime? OnlineFrom => _minecraftServerLogic.OnlineFrom;

        /// <inheritdoc/>
        public IMinecraftServerProperties Properties => _minecraftServerLogic.Properties;

        /// <inheritdoc/>
        public int Port => _minecraftServerLogic.Port;

        /// <inheritdoc/>
        public Dictionary<string, IMinecraftPlayer> Players => _minecraftServerLogic.Players;

        /// <inheritdoc/>
        public string StorageSpace => _minecraftServerLogic.StorageSpace;

        /// <inheritdoc/>
        public long StorageBytes => _minecraftServerLogic.StorageBytes;

        /// <inheritdoc/>
        public event EventHandler<ServerStatus> StatusChange
        {
            add => _minecraftServerLogic.StatusChange += value;
            remove => _minecraftServerLogic.StatusChange -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<ILogMessage> LogReceived
        {
            add => _minecraftServerLogic.LogReceived += value;
            remove => _minecraftServerLogic.LogReceived -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<IMinecraftPlayer> PlayerJoined
        {
            add => _minecraftServerLogic.PlayerJoined += value;
            remove => _minecraftServerLogic.PlayerJoined -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<IMinecraftPlayer> PlayerLeft
        {
            add => _minecraftServerLogic.PlayerLeft += value;
            remove => _minecraftServerLogic.PlayerLeft -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<(double CPU, long Memory)> PerformanceMeasured
        {
            add => _minecraftServerLogic.PerformanceMeasured += value;
            remove => _minecraftServerLogic.PerformanceMeasured -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<string> NameChanged
        {
            add => _minecraftServerLogic.NameChanged += value;
            remove => _minecraftServerLogic.NameChanged -= value;
        }
        /// <inheritdoc/>
        public event EventHandler<long> StorageMeasured
        {
            add => _minecraftServerLogic.StorageMeasured += value;
            remove => _minecraftServerLogic.StorageMeasured -= value;
        }

        /// <inheritdoc/>
        public void Shutdown(UserEventData data) =>
            _minecraftServerLogic.Shutdown(data);

        /// <inheritdoc/>
        public void Start(UserEventData data) =>
            _minecraftServerLogic.Start(data);

        /// <inheritdoc/>
        public void WriteCommand(string? command, UserEventData data)
        {
            _minecraftServerLogic.WriteCommand(command, data);
            _eventRegister.WriteCommand(Id, command!, data);
        }

    }
}
