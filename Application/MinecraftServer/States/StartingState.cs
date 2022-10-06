using Application.MinecraftServer.Enums;
using System.Text.RegularExpressions;

namespace Application.MinecraftServer.States
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
        public StartingState(MinecraftServer server) : base(server) { }

        /// <summary>
        /// Returns <see cref="ServerStatus.Starting"/>
        /// </summary>
        public override ServerStatus Status => ServerStatus.Starting;

        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool IsRunning => true;

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

        public override void Start(string username) =>
            throw new Exception(_server.ServerName + " is already starting!");

        public override void Stop(string username) =>
            throw new Exception(_server.ServerName + " is starting. Please wait till the operation is complete.");

        public override void WriteCommand(string command, string username) =>
            throw new Exception(_server.ServerName + " is starting, please wait!");
    }
}
