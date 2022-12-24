using Application.Minecraft.MinecraftServers;
using SharedPublic.Model;

namespace Application.Minecraft.States.Abstract;

internal abstract class ServerStateAbs : IServerState
{
    protected readonly MinecraftServerLogic _server;
    protected readonly object[] args;
    
    public ServerStateAbs(MinecraftServerLogic server, object[] args)
    {
        _server = server;
        this.args = args;
    }

    protected Task SetNewStateAsync<T>(params object[] args) where T : IServerState
    {
        return _server.SetServerStateAsync<T>(args);
    }

    public abstract ServerStatus Status { get; }
    public abstract bool IsRunning { get; }
    public abstract bool IsAllowedNextState(IServerState state);

    public abstract Task Apply();
    public abstract void HandleLog(LogMessage logMessage);
}
