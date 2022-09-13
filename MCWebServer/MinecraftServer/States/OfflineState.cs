using Discord;
using MCWebServer.MinecraftServer.Enums;
using System.Data.SqlTypes;
using LogMessage = MCWebServer.MinecraftServer.Enums.LogMessage;

namespace MCWebServer.MinecraftServer.States
{
    /// <summary>
    /// Represents the offline state of the minecraft server. 
    /// In this state, the process is not running, so this state usually ignores/throws exception on most of the actions.
    /// </summary>
    internal class OfflineState : IServerState
    {
        private readonly MinecraftServer _server;

        /// <summary>
        /// Initializes the Offline state, and does the offline state routine.
        /// </summary>
        /// <param name="server"></param>
        public OfflineState(MinecraftServer server)
        {
            _server = server;
            _server.StorageSpace = server.McServerProcess.GetStorage();
            _server.OnlineFrom = null;

            _server.PerformanceReporter?.Stop();
        }


        /// <summary>
        /// Returns <see cref="ServerStatus.Offline"/>
        /// </summary>
        public ServerStatus Status => ServerStatus.Offline;

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsRunning => false;



        /// <summary>
        /// Ignores all log messages, as there shouldn't be any logs during the server being offline.
        /// </summary>
        /// <param name="logMessage">this will be ignored anyways.</param>
        public void HandleLog(LogMessage logMessage) { } // do nothing, no logs while server is offline

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <param name="username">Username of the user initiated this action.</param>
        public void Start(string username)
        {
            _server.SetServerState<StartingState>();
            var logMessage = new LogMessage(username + ": " + "Starting Server", LogMessage.LogMessageType.User_Message);
            _server.AddLog(logMessage);
            _server.McServerProcess.Start();
        }

        /// <summary>
        /// Throws exception as the server is offline. 
        /// </summary>
        /// <param name="username">username of the very intelligent user who tried to stop an offline server.</param>
        /// <exception cref="Exception">Always is thrown because yeah, the serve is offline.</exception>
        public void Stop(string username) => // like who would want to stop a server when it's offline lol.
            throw new Exception("Server is already offline!");

        /// <summary>
        /// Throws exception as the server is offline.
        /// </summary>
        /// <param name="command">This parameter is ignored-</param>
        /// <param name="username">This parameter is also ignored</param>
        /// <exception cref="Exception">This is thrown always.</exception>
        public void WriteCommand(string command, string username) =>
            throw new Exception("Server is not online!");
    }
}
