using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.MinecraftServer.States
{
    internal abstract class ServerStateAbs : IServerState
    {
        protected readonly MinecraftServer _server;
        public ServerStateAbs(MinecraftServer server)
        {
            _server = server;
        }

        public abstract ServerStatus Status { get; }
        public abstract bool IsRunning { get; }

        public abstract void HandleLog(LogMessage logMessage);
        public abstract void Start(string username);
        public abstract void Stop(string username);
        public abstract void WriteCommand(string command, string username);
    }
}
