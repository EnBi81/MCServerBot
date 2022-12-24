using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using System.Text.RegularExpressions;

namespace Application.Minecraft.States;

/// <summary>
/// Represents the Starting state of the minecraft server.
/// </summary>
[ManualState]
internal class StartingState : ServerStateAbs
{
    /// <summary>
    /// Initializes the starting state.
    /// </summary>
    /// <param name="server"></param>
    public StartingState(MinecraftServerLogic server, object[] args) : base(server, args) { }

    /// <summary>
    /// Returns <see cref="ServerStatus.Starting"/>
    /// </summary>
    public override ServerStatus Status => ServerStatus.Starting;

    /// <summary>
    /// Returns true.
    /// </summary>
    public override bool IsRunning => true;

    public override async Task Apply() 
    {
        var logMessage = new LogMessage(args[0] + ": " + "Starting Server " + _server.ServerName, LogMessageType.User_Message);
        _server.AddLog(logMessage);
        await _server.StartServerProcess();
    }

    public override void HandleLog(LogMessage logMessage)
    {
        _server.AddLog(logMessage);

        var log = logMessage.Message;

        string baseTimeRegex = "\\[(\\d{2}:){2}\\d{2}\\] \\[Server thread\\/INFO\\]: ";
        Regex startupDoneRegex = new(baseTimeRegex + "Done \\([\\d.s]+\\)! For help, type \"help\"");


        // [14:02:39] [Server thread/INFO]: Done (44.552s)! For help, type "help"
        if (startupDoneRegex.IsMatch(log))
            _server.SetServerState<OnlineState>();
    }

    public override bool IsAllowedNextState(IServerState state)
    {
        return state is OnlineState or BackupAutoState;
    }
}
