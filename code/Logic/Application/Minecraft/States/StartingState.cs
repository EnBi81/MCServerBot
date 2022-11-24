using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Shared.Exceptions;
using Shared.Model;
using System.Text.RegularExpressions;

namespace Application.Minecraft.States
{
    /// <summary>
    /// Represents the Starting state of the minecraft server.
    /// </summary>
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
            if (state is OnlineState or BackupAutoState)
                return true;

            if(state is MaintenanceState)
                throw new MinecraftServerException(_server.ServerName + " is Starting up. To start Maintenance, please stop the server!");
            if(state is BackupManualState)
                throw new MinecraftServerException(_server.ServerName + " is Starting up. To start Backing up, please stop the server!");

            return false;
        }
        

        public override Task WriteCommand(string? command, string username) =>
            throw new MinecraftServerException(_server.ServerName + " is starting, please wait!");
    }
}
