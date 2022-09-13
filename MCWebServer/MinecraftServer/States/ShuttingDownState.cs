using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.MinecraftServer.States
{
    /// <summary>
    /// Represents the Shutting Down state.
    /// The server is shutting down, meaning it does not listen to any other commands. Sets all players offline.
    /// </summary>
    internal class ShuttingDownState : IServerState
    {
        private readonly MinecraftServer _server;

        /// <summary>
        /// Initializes the Shutting Down state, and does the routine
        /// </summary>
        /// <param name="server"></param>
        public ShuttingDownState(MinecraftServer server)
        {
            _server = server;
            foreach (var player in ((IMinecraftServer)server).OnlinePlayers)
            {
                server.SetPlayerOffline(player.Username);
            }
        }

        /// <summary>
        /// Returns <see cref="ServerStatus.ShuttingDown"/>
        /// </summary>
        public ServerStatus Status => ServerStatus.ShuttingDown;
        /// <summary>
        /// Returns true (because the server process is still running, even though it is not responding anymore).
        /// </summary>
        public bool IsRunning => true;



        /// <summary>
        /// Adds the log message to the log message list.
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage) =>
            _server.AddLog(logMessage);


        public void Start(string username) =>
            throw new Exception("Please wait till the server has shut down!");

        public void Stop(string username) =>
            throw new Exception("The server is shutting down!");

        public void WriteCommand(string command, string username) =>
            throw new Exception("Server is not online!");
    }
}
