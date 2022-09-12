using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.MinecraftServer.States
{
    public interface IBaseState
    {
        public ServerStatus Status { get; }
        public bool IsRunning { get; }
        public void Start(string username);
        public void Stop(string username);
        public void HandleLog(LogMessage logMessage);
        public void WriteCommand(string command, string username);
    }
}
