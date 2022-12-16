using Application.Minecraft.Backup;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Enums;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States;

/// <summary>
/// This state handles the backing up
/// </summary>
[AutoState]
internal class BackupAutoState : ServerStateAbs
{
    public BackupAutoState(MinecraftServerLogic server, object[] args) : base(server, args) { }

    public override async Task Apply()
    {
        _server.PerformanceReporter?.Stop();

        var uptimeMinutes = DateTime.Now - (_server.OnlineFrom ?? DateTime.MaxValue);
        

        if(uptimeMinutes.TotalMinutes >= _server.ServerConfig.AutoBackupAfterUptimeMinute)
        {
            _server.AddLog(new LogMessage("Auto backup server", LogMessageType.System_Message));

            var backupManager = BackupManager.Instance;

            // deleting oldest backup if limit is reached
            var backups = await backupManager.GetBackupsByServer(_server.Id);

            // all auto backups
            backups = backups.Where(b => b.Type == BackupType.Automatic);
            var limit = _server.ServerConfig.MaxAutoBackup;
            int difference = backups.Count() - limit;

            if (difference >= 0)
            {
                var oldestBackups = backups.OrderBy(b => b.CreationTime).Take(difference + 1);
                foreach(var oldBackup in oldestBackups)
                    await backupManager.DeleteBackup(oldBackup);
            }


            string backupName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_f");
            await _server.McServerFileHandler.Backup(_server.Id, backupName, BackupType.Automatic);

            _server.AddLog(new LogMessage("Auto backing up server", LogMessageType.System_Message));
        }
        else
            _server.AddLog(new LogMessage("Skipping auto backup", LogMessageType.System_Message));

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
