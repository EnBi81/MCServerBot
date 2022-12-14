using Application.Minecraft.MinecraftServers;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States.Abstract;

/// <summary>
/// Blueprint of a Minecraft Server State.
/// </summary>
internal interface IServerState
{
    /// <summary>
    /// Gets the status of the server.
    /// </summary>
    public ServerStatus Status { get; }
    /// <summary>
    /// Gets if the server process is running.
    /// </summary>
    public bool IsRunning { get; }
    /// <summary>
    /// Applies the state to the server.
    /// </summary>
    /// <param name="minecraftServer"></param>
    public Task Apply();
    /// <summary>
    /// Handles a log message, extracts data of it and calls events based on it.
    /// </summary>
    /// <param name="logMessage"></param>
    public void HandleLog(LogMessage logMessage);
    /// <summary>
    /// Returns true if the next state is allowed to set; else false. If the state is set by user, it throws an exception.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    /// <exception cref="MinecraftServerException">If the state is illegal and should not be ignored.</exception>
    public bool IsAllowedNextState(IServerState state);
}
