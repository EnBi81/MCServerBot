using Application.Minecraft.Configs;
using SharedPublic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Backup
{
    internal class BackupManager : IBackupManager
    {
        // backup folder structure: 
        //   - each server has its own folder with name of the server id
        //   - backups saved by (m|a)-name.zip, where 'm' means manual backup and 'a' means automatic backup,
        //     and 'name' is the name of the backup
        

        private readonly string _backupFolder;

        public BackupManager(string backupFolder)
        {
            this._backupFolder = backupFolder;
        }

        private DirectoryInfo GetServerBackupFolder(long serverId)
        {
            var path = Path.Combine(_backupFolder, serverId + "");
            return new DirectoryInfo(path);
        }

        public Task<IEnumerable<IBackup>> GetBackupsByServer(long serverId)
        {
            var serverBackupFolder = GetServerBackupFolder(serverId);
            if (!serverBackupFolder.Exists)
                return Task.FromResult(Enumerable.Empty<IBackup>());

            var files = serverBackupFolder.GetFiles("_-_*.zip");

            var list = new List<IBackup>();
            
            foreach(var file in files)
            {
                string backupName = file.Name[2..^file.Extension.Length];
                bool isAutomatic = file.Name[0] == 'a';

                Backup backup = new Backup
                {
                    Name = backupName,
                    IsAutomatic = isAutomatic,
                    CreationTime = file.CreationTime,
                    ServerId = serverId,
                    Size = file.Length
                };

                list.Add(backup);
            }

            return Task.FromResult(list.AsEnumerable());
        }
        
        public async Task<FileStream> CreateBackup(long serverId, string name, bool isAutomatic, MinecraftServerConfig serverConfig)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedName = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            var existingBackups = (await GetBackupsByServer(serverId)).Where(backup => backup.IsAutomatic == isAutomatic);
            int backupLimit = isAutomatic ? serverConfig.MaxAutoBackup : serverConfig.MaxManualBackup;
            
            // delete oldest backup if limit is reached
            if(existingBackups.Count() >= backupLimit)
            {
                var oldestBackup = existingBackups.OrderBy(b => b.CreationTime).FirstOrDefault();

                if (oldestBackup is not null)
                    await DeleteBackup(oldestBackup);
            }

            var backupFolder = GetServerBackupFolder(serverId);
            Directory.CreateDirectory(backupFolder.FullName);

            string backupFileName = $"{(isAutomatic ? "a" : "m")}-{sanitizedName}.zip";
            
            FileStream fs = new FileStream(backupFileName, FileMode.Create);
            return fs;
        }

        public Task DeleteBackup(IBackup backup) => throw new NotImplementedException();
        public Task<string> GetBackupPath(IBackup backup) => throw new NotImplementedException();
    }
}
