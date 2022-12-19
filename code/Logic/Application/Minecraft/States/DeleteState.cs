using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States
{
    [ManualState]
    internal class DeleteState : ServerStateAbs
    {
        public DeleteState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        public override Task Apply() 
        {
            throw new NotImplementedException();
        }

        public override ServerStatus Status => ServerStatus.Deleting;
        public override bool IsRunning => false;
        public override void HandleLog(LogMessage logMessage) { }
        public override bool IsAllowedNextState(IServerState state) => false;
        public override Task WriteCommand(string? command, string username) 
            => throw new MinecraftServerException("Server is being deleted. No commands allowed");
    }
}
