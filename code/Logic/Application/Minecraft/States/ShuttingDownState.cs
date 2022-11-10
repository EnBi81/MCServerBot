using Application.Minecraft.MinecraftServers;
using Shared.Exceptions;
using Shared.Model;

namespace Application.Minecraft.States
{
    /// <summary>
    /// Represents the Shutting Down state.
    /// The server is shutting down, meaning it does not listen to any other commands. Sets all players offline.
    /// </summary>
    internal class ShuttingDownState : ServerStateAbs
    {

        /// <summary>
        /// Initializes the Shutting Down state, and does the routine
        /// </summary>
        /// <param name="server"></param>
        public ShuttingDownState(MinecraftServerLogic server) : base(server)
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



        /// <summary>
        /// Adds the log message to the log message list.
        /// </summary>
        /// <param name="logMessage"></param>
        public override void HandleLog(LogMessage logMessage) =>
            _server.AddLog(logMessage);


        public override Task Start(string username) =>
            throw new MinecraftServerException($"Please wait till {_server.ServerName} has shut down!");

        public override Task Stop(string username) =>
            throw new MinecraftServerException($"{_server.ServerName} is shutting down!");

        public override Task WriteCommand(string? command, string username) =>
            throw new MinecraftServerException(_server.ServerName + " is not online!");
    }
}
