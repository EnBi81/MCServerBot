using Discord;
using MCWebServer.MinecraftServer.Enums;
using System.Data.SqlTypes;
using LogMessage = MCWebServer.MinecraftServer.Enums.LogMessage;

namespace MCWebServer.MinecraftServer.States
{
    internal class OfflineState : IBaseState
    {
        private readonly MinecraftServer _server;

        public OfflineState(MinecraftServer server)
        {
            _server = server;
            _server.StorageSpace = server.McServerProcess.GetStorage();
            _server.OnlineFrom = null;

            _server.PerformanceReporter?.Stop();
        }





        public ServerStatus Status => ServerStatus.Offline;

        public bool IsRunning => false;




        public void HandleLog(LogMessage logMessage)
        {
            // do nothing, no logs while server is offline
        }

        public void Start(string username)
        {
            _server.SetServerState<StartingState>();
            var logMessage = new LogMessage(username + ": " + "Starting Server", LogMessage.LogMessageType.User_Message);
            _server.AddLog(logMessage);
            _server.McServerProcess.Start();
        }

        public void Stop(string username)
        {
            throw new Exception("Server is already offline!");
        }

        public void WriteCommand(string command, string username)
        {
            throw new Exception("Server is not online!");
        }
    }
}
