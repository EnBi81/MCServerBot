using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States
{
    [ManualState]
    internal class RestoreState : ServerStateAbs
    {
        public RestoreState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        public override ServerStatus Status => ServerStatus.Restore;

        public override bool IsRunning => false;

        public override async Task Apply()
        {
            if (args is { Length: 0} || args[0] is not IBackup backup)
                throw new MCInternalException("No backup present when restoring");


            _server.McServerFileHandler.BackUpImportantFiles();
            await _server.McServerFileHandler.RestoreBackup(_server.Id, backup);
            _server.McServerFileHandler.DeleteTemporaryBackupFolder();

            await _server.SetServerStateAsync<OfflineState>();
        }
        public override void HandleLog(LogMessage logMessage) { }
        public override bool IsAllowedNextState(IServerState state) => state is OfflineState;
        public override Task WriteCommand(string? command, string username) => throw new MinecraftServerException(_server.ServerName + " is restoring!");
    }
}
