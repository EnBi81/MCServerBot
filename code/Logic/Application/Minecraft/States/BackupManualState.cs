using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using Shared.Exceptions;
using Shared.Model;
namespace Application.Minecraft.States
{
    [ManualState]
    internal class BackupManualState : ServerStateAbs
    {
        public BackupManualState(MinecraftServerLogic server, string[] args) : base(server, args) { }

        public override async Task Apply()
        {
            if (args[0] is not string backupName)
                throw new MCInternalException("Invalid backup arguments");


            await _server.McServerFileHandler.Backup(_server.Id, backupName, false);
            await _server.SetServerStateAsync<OfflineState>();
        }



        public override ServerStatus Status => ServerStatus.BackUp;

        public override bool IsRunning => false;

        public override bool IsAllowedNextState(IServerState state)
        {
            return state is OfflineState;
        }

        public override void HandleLog(LogMessage logMessage) { }

        public override Task WriteCommand(string? command, string username) =>
            throw new MinecraftServerException(_server.ServerName + " is backing up!");
    }
}
