using Microsoft.VisualBasic.FileIO;
using SharedPublic.Model;

namespace Application.Minecraft.Backup
{
    internal class BackupManager : IBackupManager
    {
        // backup folder structure: 
        //   - each server has its own folder with name of the server id
        //   - backups saved by (m|a)-name.zip, where 'm' means manual backup and 'a' means automatic backup,
        //     and 'name' is the name of the backup


        private static BackupManager? _instance;
        public static BackupManager Instance => _instance ?? throw new InvalidOperationException("BackupManager is not initialized.");

        public static void Initialize(string backupFolder)
        {
            if (_instance is not null)
                return;

            _instance = new BackupManager(backupFolder);
        }





        private readonly string _backupFolder;

        public BackupManager(string backupFolder)
        {
            _backupFolder = backupFolder;
        }

        private DirectoryInfo GetServerBackupFolder(long serverId)
        {
            var path = Path.Combine(_backupFolder, serverId + "");
            return new DirectoryInfo(path);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IBackup>> GetBackupsByServer(long serverId)
        {
            var serverBackupFolder = GetServerBackupFolder(serverId);
            if (!serverBackupFolder.Exists)
                return Task.FromResult(Enumerable.Empty<IBackup>());

            var files = serverBackupFolder.GetFiles("?-?*.zip");

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

        /// <inheritdoc/>
        public Task<string> CreateBackupPath(long serverId, string name, bool isAutomatic)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedName = string.Join("_", name.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');

            var backupFolder = GetServerBackupFolder(serverId);
            Directory.CreateDirectory(backupFolder.FullName);

            string backupFileName = $"{(isAutomatic ? "a" : "m")}-{sanitizedName}.zip";

            return Task.FromResult(Path.Combine(backupFolder.FullName, backupFileName));
        }

        /// <inheritdoc/>
        public Task DeleteBackup(IBackup backup)
        {
            string backupFileName = $"1\\{(backup.IsAutomatic ? "a" : "m")}-{backup.Name}.zip";
            string backupFileFullPath = Path.Combine(_backupFolder, backupFileName);

            FileSystem.DeleteFile(backupFileFullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);

            return Task.CompletedTask;
        }
    }
}
