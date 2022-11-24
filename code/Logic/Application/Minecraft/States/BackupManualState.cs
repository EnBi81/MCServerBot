using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Shared.Exceptions;
using Shared.Model;
namespace Application.Minecraft.States
{
    internal class BackupManualState : ServerStateAbs
    {
        public BackupManualState(MinecraftServerLogic server, string[] args) : base(server, args) { }

        public override Task Apply()
        {
            if (args[0] is not string backupName)
                throw new MCInternalException("Invalid backup arguments");

            // TODO: backup
            // https://stackoverflow.com/questions/35415936/how-to-zip-a-directorys-contents-except-one-subdirectory
            return Task.CompletedTask;
        }



        public override ServerStatus Status => ServerStatus.BackUp;

        public override bool IsRunning => false;

        public override bool IsAllowedNextState(IServerState state)
        {
            if (state is not OfflineState)
                throw new MinecraftServerException(_server.ServerName + " is backing up. Please wait!");

            return true;
        }

        public override void HandleLog(LogMessage logMessage) { }

        public override Task WriteCommand(string? command, string username) =>
            throw new MinecraftServerException(_server.ServerName + " is backing up!");
    }
}
