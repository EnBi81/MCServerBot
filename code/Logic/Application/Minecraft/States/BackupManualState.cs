using Application.Minecraft.Backup;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Enums;
using SharedPublic.Exceptions;
using SharedPublic.Model;
namespace Application.Minecraft.States
{
    [ManualState]
    internal class BackupManualState : ServerStateAbs
    {
        public BackupManualState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        public override async Task Apply()
        {
            if (args[0] is not string backupName)
            {
                throw new MCInternalException("Invalid backup arguments");
            }

            var backupManager = BackupManager.Instance;

            // deleting oldest backup if limit is reached
            var backups = await backupManager.GetBackupsByServer(_server.Id);

            // all auto backups
            backups = backups.Where(b => b.Type == BackupType.Manual);
            var limit = _server.ServerConfig.MaxAutoBackup;
            int difference = backups.Count() - limit;

            if (difference >= 0)
            {
                var oldestBackups = backups.OrderBy(b => b.CreationTime).Take(difference + 1);
                foreach (var oldBackup in oldestBackups)
                    await backupManager.DeleteBackup(oldBackup);
            }

            await _server.McServerFileHandler.Backup(_server.Id, backupName, BackupType.Manual);
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
