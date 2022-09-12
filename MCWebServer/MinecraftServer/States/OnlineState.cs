using Discord;
using MCWebServer.Log;
using MCWebServer.MinecraftServer.Enums;
using System.Text.RegularExpressions;
using LogMessage = MCWebServer.MinecraftServer.Enums.LogMessage;

namespace MCWebServer.MinecraftServer.States
{
    internal class OnlineState : IServerState
    {
        private readonly MinecraftServer _server;

        public OnlineState(MinecraftServer server)
        {
            _server = server;
            _server.OnlineFrom = DateTime.Now;
        }



        public ServerStatus Status => ServerStatus.Online;

        public bool IsRunning => true;

        public void HandleLog(LogMessage logMessage)
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

        public void Start(string username) =>
            throw new Exception("Server is already running!");

        public void Stop(string username)
        {
            LogService.GetService<MinecraftLogger>().Log("server", $"Shutdown request by: " + username);
            WriteCommand("stop", username);
        }

        public void WriteCommand(string command, string username)
        {
            _server.McServerProcess.WriteToStandardInput(command);
            var logMess = new LogMessage(username + ": " + command, LogMessage.LogMessageType.User_Message);
            _server.AddLog(logMess);
        }
    }
}
