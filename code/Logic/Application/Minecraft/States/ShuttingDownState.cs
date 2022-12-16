using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States;

/// <summary>
/// Represents the Shutting Down state.
/// The server is shutting down, meaning it does not listen to any other commands. Sets all players offline.
/// </summary>
[ManualState]
internal class ShuttingDownState : ServerStateAbs
{

    /// <summary>
    /// Initializes the Shutting Down state, and does the routine
    /// </summary>
    /// <param name="server"></param>
    public ShuttingDownState(MinecraftServerLogic server, object[] args) : base(server, args)
    {
        foreach (var player in ((IMinecraftServer)server).OnlinePlayers)
        {
            server.SetPlayerOffline(player.Username);
        }
    }

    /// <summary>
    /// Returns <see cref="ServerStatus.ShuttingDown"/>
    /// </summary>
    public override ServerStatus Status => ServerStatus.ShuttingDown;
    /// <summary>
    /// Returns true (because the server process is still running, even though it is not responding anymore).
    /// </summary>
    public override bool IsRunning => true;

    public override bool IsAllowedNextState(IServerState state)
    {
        return state is BackupAutoState;
    }

    public override async Task Apply()
    {
        if (_server.McServerProcess.IsRunning)
            await _server.McServerProcess.WriteToStandardInputAsync("stop");
        else
            await _server.SetServerStateAsync<BackupAutoState>();
    }

    /// <summary>
    /// Adds the log message to the log message list.
    /// </summary>
    /// <param name="logMessage"></param>
    public override void HandleLog(LogMessage logMessage) =>
        _server.AddLog(logMessage);
    

    public override Task WriteCommand(string? command, string username) =>
        throw new MinecraftServerException(_server.ServerName + " is not online!");
    
}
