using Shared.Exceptions;
using Shared.Model;
using System.Text.RegularExpressions;
using static Shared.Model.ILogMessage;
using LogMessage = Application.Minecraft.MinecraftServers.LogMessage;

namespace Application.Minecraft.States
{
    /// <summary>
    /// Represents the Online state of the minecraft server.
    /// In this state, the process is running and ready for all type of user interaction.
    /// </summary>
    internal class OnlineState : ServerStateAbs
    {

        /// <summary>
        /// Initializes the Online state, and does the online state routine.
        /// </summary>
        /// <param name="server"></param>
        public OnlineState(MinecraftServerLogic server) : base(server)
        {
            _server.OnlineFrom = DateTime.Now;
        }


        /// <summary>
        /// Returns <see cref="ServerStatus.Online"/>
        /// </summary>
        public override ServerStatus Status => ServerStatus.Online;

        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool IsRunning => true;

        /// <summary>
        /// Handles the log message.
        /// </summary>
        /// <param name="logMessage">The log message to be handled</param>
        public override void HandleLog(LogMessage logMessage)
        {
            _server.AddLog(logMessage);

            var log = logMessage.Message;

            string baseTimeRegex = "\\[(\\d{2}:){2}\\d{2}\\] \\[Server thread\\/INFO\\]: ";
            Regex playerJoinedRegex = new(baseTimeRegex + "([a-zA-Z0-9_]+) joined the game");
            Regex playerLeftRegex = new(baseTimeRegex + "([a-zA-Z0-9_]+) left the game");
            Regex shutdownRegex = new(baseTimeRegex + "Stopping the server");


            // [21:34:35] [Server thread/INFO]: Enbi81 joined the game
            if (playerJoinedRegex.IsMatch(log))
            {
                var match = playerJoinedRegex.Match(log);
                var cap = match.Groups[2];

                var username = cap.Value;
                _server.SetPlayerOnline(username);
            }

            // [21:35:08] [Server thread/INFO]: Enbi81 left the game
            else if (playerLeftRegex.IsMatch(log))
            {
                var match = playerLeftRegex.Match(log);
                var cap = match.Groups[2];

                var username = cap.Value;
                _server.SetPlayerOffline(username);
            }

            else if (shutdownRegex.IsMatch(log))
            {
                _server.SetServerState<ShuttingDownState>();
            }
        }

        /// <summary>
        /// Throws exception as the server cant be started; it is already online.
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="Exception"></exception>
        public override Task Start(string username) =>
            throw new MinecraftServerException(_server.ServerName + " is already running!");

        /// <summary>
        /// Stops the server by writing stop to the server process window.
        /// </summary>
        /// <param name="username"></param>
        public override async Task Stop(string username)
        {
            await WriteCommand("stop", username);
        }

        /// <summary>
        /// Writes command to the server process.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="username"></param>
        public override async Task WriteCommand(string? command, string username)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new MinecraftServerArgumentException(nameof(command) + " command must not be null or empty.");

            await _server.McServerProcess.WriteToStandardInputAsync(command);
            var logMess = new LogMessage(_server.ServerName + "/" + username + ": " + command, LogMessageType.User_Message);
            _server.AddLog(logMess);
        }
    }
}
