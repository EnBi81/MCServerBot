using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.MinecraftServer.States
{
    internal class ShuttingDownState : IBaseState
    {
        private readonly MinecraftServer _server;

        public ShuttingDownState(MinecraftServer server)
        {
            foreach (var player in server.OnlinePlayers)
            {
                server.SetPlayerOffline(player.Username);
            }
        }


        public ServerStatus Status => ServerStatus.ShuttingDown;

        public bool IsRunning => true;




        public void HandleLog(LogMessage logMessage)
        {
            _server.AddLog(logMessage);
        }

        public void Start(string username)
        {
            throw new Exception("Please wait till the server has shut down!");
        }

        public void Stop(string username)
        {
            throw new Exception("The server is shutting down!");
        }

        public void WriteCommand(string command, string username)
        {
            throw new Exception("Server is not online!");
        }
    }
}
