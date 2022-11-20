using Application.Minecraft.MinecraftServers;
using Shared.Model;

namespace Application.Minecraft.States.Abstract
{
    internal abstract class ServerStateAbs : IServerState
    {
        protected readonly MinecraftServerLogic _server;
        protected readonly string[] args;
        
        public ServerStateAbs(MinecraftServerLogic server, string[] args)
        {
            _server = server;
            this.args = args;
        }

        public abstract ServerStatus Status { get; }
        public abstract bool IsRunning { get; }
        public abstract bool IsAllowedNextState(IServerState state);

        public abstract Task Apply();
        public abstract void HandleLog(LogMessage logMessage);
        public abstract Task WriteCommand(string? command, string username);
    }
}
