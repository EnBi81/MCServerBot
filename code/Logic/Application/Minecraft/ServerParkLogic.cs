﻿using APIModel.DTOs;
using Application.DAOs;
using Application.Minecraft.Backup;
using Application.Minecraft.Configs;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.Util;
using Loggers;
using SharedPublic.DTOs;
using SharedPublic.EventHandlers;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using System.Collections.ObjectModel;

namespace Application.Minecraft;

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
    


    private long _serverIdCounter;
    private readonly IDatabaseAccess _databaseAccess;
    private readonly MinecraftConfig _config;
    private readonly MinecraftLogger _logger;
    private readonly MinecraftEnvironment _mcEnvironment;


    internal ServerParkLogic(IDatabaseAccess dataAccess, MinecraftConfig config, MinecraftLogger logger)
    {
        _databaseAccess = dataAccess;
        _config = config;
        _logger = logger;
        _mcEnvironment = new MinecraftEnvironment(config.MinecraftServersBaseFolder);

        Versions.MinecraftVersionCollection.Initialize(_mcEnvironment.ServerJarDirectory, _logger);
        Backup.BackupManager.Initialize(config.BackupFolder);


        ServersFolder = _config.MinecraftServersBaseFolder + "Servers\\";
        DeletedServersFolder = _config.MinecraftServersBaseFolder + "DeletedServers\\";

        
        ActiveServerPlayerLeft = null!;
        ActiveServerPlayerJoined = null!;
        ActiveServerLogReceived = null!;
        ActiveServerPerformanceMeasured = null!;
        ActiveServerStatusChange = null!;

        ServerAdded = null!;
        ServerDeleted = null!;
        ServerModified = null!;
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        _serverIdCounter = await _databaseAccess.ServerParkDataAccess.GetMaxServerId();
        await MinecraftVersionCollection.InitializeAsync();

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

    /// <inheritdoc/>
    public IMinecraftVersionCollection MinecraftVersionCollection => Versions.MinecraftVersionCollection.Instance;

    /// <inheritdoc/>
    public IBackupManager BackupManager => Backup.BackupManager.Instance;


    /// <summary>
    /// List of all minecraft server instances
    /// </summary>
    internal Dictionary<long, IMinecraftServer> ServerCollection { get; } = new();
    


    /// <inheritdoc/>
    public IMinecraftServer GetServer(long id)
    {
        if (ServerCollection.TryGetValue(id, out IMinecraftServer? server))
            return server;
         
        throw new MCExternalException($"ERROR: Server with id {id} not found!");
    }
        



    /// <inheritdoc/>
    private void SetActiveServer(long id)
    {
        IMinecraftServer server = GetServer(id);

        if (ActiveServer == server)
            return;

        UnSubscribeEventTrackers(ActiveServer);
        ActiveServer = server;
        SubscribeEventTrackers(ActiveServer);
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
    public Task<IMinecraftServer> CreateServer(ServerCreationDto dto, UserEventData user)
    {
        var serverName = dto.NewName!;

        var version = MinecraftVersionCollection[dto.Version] ?? MinecraftVersionCollection.Latest;

        IMinecraftServer mcServer;
        // synchronization increment
        lock (this)
        {
            long newServerId = Interlocked.Increment(ref _serverIdCounter);

            string destDir = ServersFolder + newServerId;
            Directory.CreateDirectory(destDir);

            mcServer = new MinecraftServer(_databaseAccess.MinecraftDataAccess,
                _logger, newServerId, serverName,
                destDir, _config, version, dto.Properties);

            RegisterMcServer(mcServer);
        }
        
        return Task.FromResult(mcServer);
    }


    /// <inheritdoc/>
    public Task<IMinecraftServer> ModifyServer(long id, ModifyServerDto dto, UserEventData user)
    {
        var server = GetServer(id);
        
        if (dto.Version != null)
            server.MCVersion = MinecraftVersionCollection[dto.Version]!;
        
        if (dto.NewName != null)
            server.ServerName = dto.NewName;

        if(dto.Properties != null)
            server.Properties.UpdatePropertiesAsync(dto.Properties);

        InvokeServerModified(server, dto);

        return Task.FromResult(server);
    }

    /// <inheritdoc/>
    public Task<IMinecraftServer> DeleteServer(long id, UserEventData user)
    {
        string newDir = $"{DeletedServersFolder}{id}-{DateTime.Now:yyyy-MM-dd HH-mm-ss}";
        
        FileHelper.MoveDirectory(ServersFolder + id, newDir);
        Task.Run(() =>
        {
            FileHelper.ZipDirectory(newDir, newDir + ".zip");
            FileHelper.DeleteDirectory(newDir);
        });

        ServerCollection.Remove(id, out IMinecraftServer? server);

        if (server != null)
            InvokeServerDeleted(server);

        return Task.FromResult(server!);
    }

    /// <summary>
    /// Register a new minecraft server object to the program.
    /// </summary>
    private void RegisterMcServer(IMinecraftServer server)
    {
        ServerCollection.Add(server.Id, server);
        InvokeServerAdded(server);
    }
    

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
    public event EventHandler<ServerValueEventArgs<ModifyServerDto>> ServerModified;

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
    private void InvokeServerModified(IMinecraftServer server, ModifyServerDto dto) =>
        ServerModified?.Invoke(this, new(dto, server));
    private void InvokeServerAdded(IMinecraftServer addedServer) =>
        ServerAdded?.Invoke(this, new(addedServer));
    private void InvokeServerDeleted(IMinecraftServer deletedServer) =>
        ServerDeleted?.Invoke(this, new(deletedServer));

}
