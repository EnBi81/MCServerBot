using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using LogMessage = Application.Minecraft.MinecraftServers.LogMessage;

namespace Application.Minecraft.States
{
    /// <summary>
    /// Represents the offline state of the minecraft server. 
    /// In this state, the process is not running, so this state usually ignores/throws exception on most of the actions.
    /// </summary>
    [AutoState]
    internal class OfflineState : ServerStateAbs
    {
        
        /// <summary>
        /// Initializes the Offline state, and does the offline state routine.
        /// </summary>
        /// <param name="server"></param>
        public OfflineState(MinecraftServerLogic server, object[] args) : base(server, args)
        {
            
        }

        public override Task Apply()
        {
            var serverUptime = DateTime.Now - (_server.OnlineFrom ?? DateTime.MaxValue);
            if(serverUptime.TotalSeconds > 0)
                _server.AddLog(new LogMessage("Server online for: " + serverUptime, LogMessageType.System_Message));

            _server.StorageBytes = _server.McServerProcess.GetStorage();
            _server.OnlineFrom = null;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns <see cref="ServerStatus.Offline"/>
        /// </summary>
        public override ServerStatus Status => ServerStatus.Offline;

        /// <summary>
        /// Returns false.
        /// </summary>
        public override bool IsRunning => false;
        
        public override bool IsAllowedNextState(IServerState state)
        {
            return state is StartingState or BackupManualState or MaintenanceState;
        }

        /// <summary>
        /// Ignores all log messages, as there shouldn't be any logs during the server being offline.
        /// </summary>
        /// <param name="logMessage">this will be ignored anyways.</param>
        public override void HandleLog(LogMessage logMessage) { } // do nothing, no logs while server is offline
        
        /// <summary>
        /// Throws exception as the server is offline.
        /// </summary>
        /// <param name="command">This parameter is ignored-</param>
        /// <param name="username">This parameter is also ignored</param>
        /// <exception cref="Exception">This is thrown always.</exception>
        public override Task WriteCommand(string? command, string username) =>
            throw new MinecraftServerException(_server.ServerName + " is not online!");
    }
}
