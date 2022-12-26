using APIModel.DTOs;
using Application.DAOs;
using Application.DAOs.Database;
using Application.Minecraft.Configs;
using Loggers;
using SharedPublic.DTOs;
using SharedPublic.EventHandlers;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft;


/// <summary>
/// Proxy object for ServerPark, it handles all the database registrations.
/// </summary>
public class ServerPark : IServerPark
{
    private readonly IServerParkDataAccess _serverParkEventRegister;
    private readonly IServerPark _serverPark;
    private readonly MinecraftLogger _logger;

    private bool _initialized = false;

    public ServerPark(IDatabaseAccess databaseAccess, MinecraftConfig config, MinecraftLogger logger)
    {
        _serverParkEventRegister = databaseAccess.ServerParkDataAccess;
        _logger = logger;

        _logger.Log(_logger.ServerPark, $"Initializing Serverpark");
        try
        {
            _serverPark = new ServerParkInputValidation(databaseAccess, config, logger);
        }
        catch (Exception e)
        {
            logger.Error(logger.ServerPark, e);
            throw;
        }

        _logger.Log(_logger.ServerPark, $"Serverpark initialized");
    }


    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        await _serverPark.InitializeAsync();
        _initialized = true;

        SetupLogging();
    }

    private void SetupLogging()
    {
        _serverPark.ServerAdded += (s, e) => _logger.Log(_logger.ServerPark, $"{e.NewValue.Id}:{e.NewValue.ServerName} created");
        _serverPark.ServerDeleted += (s, e) => _logger.Log(_logger.ServerPark, $"{e.NewValue.Id}:{e.NewValue.ServerName} deleted");
    }

    /// <summary>
    /// Throws an exception if the instance is not initialized.
    /// </summary>
    /// <exception cref="Exception">when the instance is not initialized.</exception>
    private void ThrowExceptionIfNotInitialized()
    {
        if (!_initialized)
            throw new MCInternalException("ServerPark not initialized!");
    }

    /// <inheritdoc/>
    public IMinecraftServer? ActiveServer
    {
        get
        {
            ThrowExceptionIfNotInitialized();
            return _serverPark.ActiveServer;
        }
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<long, IMinecraftServer> MCServers
    {
        get
        {
            ThrowExceptionIfNotInitialized();
            return _serverPark.MCServers;
        }
    }

    /// <inheritdoc/>
    public IMinecraftVersionCollection MinecraftVersionCollection
    {
        get
        {
            ThrowExceptionIfNotInitialized();
            return _serverPark.MinecraftVersionCollection;
        }
    }

    /// <inheritdoc/>
    public IBackupManager BackupManager
    {
        get
        {
            ThrowExceptionIfNotInitialized();
            return _serverPark.BackupManager;
        }
    }

    /// <inheritdoc/>
    public event EventHandler<ServerValueEventArgs<ServerStatus>> ActiveServerStatusChange
    {
        add
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ActiveServerStatusChange += value;
        }
        remove
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ActiveServerStatusChange -= value;
        }
    }
    /// <inheritdoc/>
    public event EventHandler<ServerValueEventArgs<ILogMessage>> ActiveServerLogReceived
    {
        add
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ActiveServerLogReceived += value;
        }
        remove
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ActiveServerLogReceived -= value;
        }
    }
    /// <inheritdoc/>
    public event EventHandler<ServerValueEventArgs<(double CPU, long Memory)>> ActiveServerPerformanceMeasured
    {
        add
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ActiveServerPerformanceMeasured += value;
        }
        remove
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ActiveServerPerformanceMeasured -= value;
        }
    }
    /// <inheritdoc/>
    public event EventHandler<ServerValueEventArgs<ModifyServerDto>> ServerModified
    {
        add
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ServerModified += value;
        }
        remove
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ServerModified -= value;
        }
    }
    /// <inheritdoc/>
    public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerAdded
    {
        add
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ServerAdded += value;
        }
        remove
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ServerAdded -= value;
        }
    }
    /// <inheritdoc/>
    public event EventHandler<ValueEventArgs<IMinecraftServer>> ServerDeleted
    {
        add
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ServerDeleted += value;
        }
        remove
        {
            ThrowExceptionIfNotInitialized();
            _serverPark.ServerDeleted -= value;
        }
    }



    /// <inheritdoc/>
    public async Task<IMinecraftServer> CreateServer(ServerCreationDto dto, UserEventData user)
    {
        ThrowExceptionIfNotInitialized();

        var res = await _serverPark.CreateServer(dto, user);
        await _serverParkEventRegister.CreateServer(res.Id, res.ServerName, user);

        return res;
    }

    /// <inheritdoc/>
    public async Task<IMinecraftServer> DeleteServer(long id, UserEventData user)
    {
        ThrowExceptionIfNotInitialized();

        var server = await _serverPark.DeleteServer(id, user);
        await _serverParkEventRegister.DeleteServer(server.Id, user);

        return server;
    }

    /// <inheritdoc/>
    public async Task<IMinecraftServer> ModifyServer(long id, ModifyServerDto dto, UserEventData user)
    {
        ThrowExceptionIfNotInitialized();

        var server = await _serverPark.ModifyServer(id, dto, user);
        if(dto.NewName is not null)
            await _serverParkEventRegister.RenameServer(server.Id, dto.NewName, user);

        return server;
    }

    /// <inheritdoc/>
    public async Task StartServer(long id, UserEventData user)
    {
        ThrowExceptionIfNotInitialized();

        await _serverPark.StartServer(id, user);

        var server = ActiveServer;
        await _serverParkEventRegister.StartServer(server!.Id, user);
    }

    /// <inheritdoc/>
    public async Task StopActiveServer(UserEventData user)
    {
        ThrowExceptionIfNotInitialized();

        await _serverPark.StopActiveServer(user);

        var server = ActiveServer;
        await _serverParkEventRegister.StopServer(server!.Id, user);
    }

    /// <inheritdoc/>
    public async Task ToggleServer(long id, UserEventData user)
    {
        ThrowExceptionIfNotInitialized();

        bool isRunning = ActiveServer?.IsRunning ?? false;

        await _serverPark.ToggleServer(id, user);

        var server = ActiveServer;
        if (isRunning)
            await _serverParkEventRegister.StopServer(server!.Id, user);
        else
            await _serverParkEventRegister.StartServer(server!.Id, user);
    }

    /// <inheritdoc/>
    public IMinecraftServer GetServer(long id)
    {
        ThrowExceptionIfNotInitialized();
        return _serverPark.GetServer(id);
    }
}
