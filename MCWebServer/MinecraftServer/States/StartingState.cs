using MCWebServer.MinecraftServer.Enums;
using System.Text.RegularExpressions;

namespace MCWebServer.MinecraftServer.States
{
    /// <summary>
    /// Represents the Starting state of the minecraft server.
    /// </summary>
    internal class StartingState : IServerState
    {
        private readonly MinecraftServer _server;

        /// <summary>
        /// Initializes the starting state.
        /// </summary>
        /// <param name="server"></param>
        public StartingState(MinecraftServer server)
        {
            _server = server;
        }

        /// <summary>
        /// Returns <see cref="ServerStatus.Starting"/>
        /// </summary>
        public ServerStatus Status => ServerStatus.Starting;

        /// <summary>
        /// Returns true.
        /// </summary>
        public bool IsRunning => true;

        public void HandleLog(LogMessage logMessage)
        {
            _server.AddLog(logMessage);

            var log = logMessage.Message;

            string baseTimeRegex = "\\[(\\d{2}:){2}\\d{2}\\] \\[Server thread\\/INFO\\]: ";
            Regex startupDoneRegex = new(baseTimeRegex + "Done \\([\\d.s]+\\)! For help, type \"help\"");


            // [14:02:39] [Server thread/INFO]: Done (44.552s)! For help, type "help"
            if (startupDoneRegex.IsMatch(log))
                _server.SetServerState<OnlineState>();
        }

        public void Start(string username) =>
            throw new Exception("Server is already starting!");

        public void Stop(string username) =>
            throw new Exception("Server is starting. Please wait till the operation is complete.");

        public void WriteCommand(string command, string username) =>
            throw new Exception("Server is starting, please wait!");
    }
}
