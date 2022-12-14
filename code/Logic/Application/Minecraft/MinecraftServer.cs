using Application.DAOs.Database;
using Application.Minecraft.Configs;
using Application.Minecraft.DTOs;
using Application.Minecraft.Versions;
using Loggers;
using SharedPublic.DTOs;
using SharedPublic.Model;
using System.Text.Json.Serialization;

namespace Application.Minecraft;

/// <summary>
/// Proxy class for logging and handling database records.
/// </summary>
internal class MinecraftServer : IMinecraftServer
{
    private readonly MinecraftLogger _logger;
    private readonly IMinecraftDataAccess _eventRegister;
    private readonly IMinecraftServer _minecraftServerLogic;

    

    public MinecraftServer(ExistingServerCreationDto dto) : this(dto.DataAccess, dto.Logger)
    {
        _minecraftServerLogic = InitLogicServer(() => new MinecraftServerLogic(dto));
        _logger.Log(_logger.MinecraftServer, $"Server {ServerName} created");
        Startup();
    }


    public MinecraftServer(NewServerCreationDto dto) : this(dto.DataAccess, dto.Logger)
    {
        _minecraftServerLogic = InitLogicServer(() => new MinecraftServerLogic(dto));
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
            => _logger.Log(mcServer + "-performance", $"{Id}:{ServerName} measurement: CPU - {e.CPU:0.00}%  Memory - {e.Memory / (1024 * 1024)} MB");
        _minecraftServerLogic.PlayerJoined += (s, e)
            => _logger.Log(mcServer + "-player", $"{e.Username} joined {Id}:{ServerName}");
        _minecraftServerLogic.PlayerLeft += (s, e)
            => _logger.Log(mcServer + "-player", $"{e.Username} left {Id}:{ServerName}");
        _minecraftServerLogic.StatusChange += (s, e)
            => _logger.Log(mcServer + "-status", $"{Id}:{ServerName} new status: " + e.DisplayString());
        _minecraftServerLogic.StorageMeasured += (s, e)
            => _logger.Log(mcServer + "-storage", $"{Id}:{ServerName} storage measured: {e / (1024 * 1024)} MB");
        _minecraftServerLogic.NameChanged += (s, e)
            => _logger.Log(mcServer, $"{Id}:{ServerName} name changed to {ServerName}");
        _minecraftServerLogic.VersionChanged += (s, e)
            => _logger.Log(mcServer + "-version", $"{Id}:{ServerName} version changed to {e.Version}");
    }

    /// <inheritdoc/>
    public long Id => _minecraftServerLogic.Id;

    /// <inheritdoc/>
    public string ServerName { get => _minecraftServerLogic.ServerName; }

    /// <inheritdoc/>
    public ServerStatus StatusCode => _minecraftServerLogic.StatusCode;

    /// <inheritdoc/>
    public bool IsRunning => _minecraftServerLogic.IsRunning;

    /// <inheritdoc/>
    public ICollection<ILogMessage> Logs => _minecraftServerLogic.Logs;

    /// <inheritdoc/>
    public DateTime? OnlineFrom => _minecraftServerLogic.OnlineFrom;

    /// <inheritdoc/>
    [JsonIgnore]
    public IMinecraftServerProperties Properties => _minecraftServerLogic.Properties;

    /// <inheritdoc/>
    public int Port => _minecraftServerLogic.Port;

    /// <inheritdoc/>
    public string StorageSpace => _minecraftServerLogic.StorageSpace;

    /// <inheritdoc/>
    public long StorageBytes => _minecraftServerLogic.StorageBytes;

    /// <inheritdoc/>
    public IMinecraftVersion MCVersion { get => _minecraftServerLogic.MCVersion; }

    /// <inheritdoc/>
    [JsonIgnore]
    public string? ServerIcon { get => _minecraftServerLogic.ServerIcon; }

    /// <inheritdoc/>
    [JsonIgnore]
    public IDictionary<string, IPlayerFull> PlayersFull => _minecraftServerLogic.PlayersFull;

    public IEnumerable<IPlayerSimple> Players => _minecraftServerLogic.Players;

    /// <inheritdoc/>
    public event EventHandler<IMinecraftServer> Deleted
    {
        add => _minecraftServerLogic.Deleted += value;
        remove => _minecraftServerLogic.Deleted -= value;
    }
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
    public event EventHandler<IPlayerSimple> PlayerJoined
    {
        add => _minecraftServerLogic.PlayerJoined += value;
        remove => _minecraftServerLogic.PlayerJoined -= value;
    }
    /// <inheritdoc/>
    public event EventHandler<IPlayerSimple> PlayerLeft
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
    public event EventHandler<IMinecraftVersion> VersionChanged
    {
        add => _minecraftServerLogic.VersionChanged += value;
        remove => _minecraftServerLogic.VersionChanged -= value;
    }

    /// <inheritdoc/>
    public Task Shutdown(UserEventData data) =>
        _minecraftServerLogic.Shutdown(data);

    /// <inheritdoc/>
    public Task Start(UserEventData data) =>
        _minecraftServerLogic.Start(data);

    /// <inheritdoc/>
    public Task<CommandResponse> WriteCommand(string? command, UserEventData data)
    {
        var task = _minecraftServerLogic.WriteCommand(command, data);
        _eventRegister.WriteCommand(Id, command!, data);
        return task;
    }

    /// <inheritdoc/>
    public Task Backup(BackupDto dto, UserEventData data = default)
    {
        return _minecraftServerLogic.Backup(dto, data);
    }

    /// <inheritdoc/>
    public Task Restore(IBackup backup, UserEventData data = default)
    {
        return _minecraftServerLogic.Restore(backup, data);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(UserEventData data = default)
    {
        return _minecraftServerLogic.DeleteAsync(data);
    }

    /// <inheritdoc/>
    public Task ModifyAsync(ModifyServerDto dto, UserEventData user)
    {
        return _minecraftServerLogic.ModifyAsync(dto, user);
    }
}
