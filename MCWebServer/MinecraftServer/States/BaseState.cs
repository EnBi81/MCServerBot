using MCWebServer.MinecraftServer.Enums;

namespace MCWebServer.MinecraftServer.States
{
    public interface IBaseState
    {
        public ServerStatus Status { get; }
        public bool IsRunning { get; }
        public void ToggleServer();
        public void HandleLog(LogMessage logMessage);
        public void Shutdown();
        public void WriteCommand();
    }
}
