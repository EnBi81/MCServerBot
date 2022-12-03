using Application.Minecraft.Backup;
using Application.Minecraft.Configs;
using Application.Minecraft.Util;
using SharedPublic.Enums;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.MinecraftServers.Utils
{
    internal class MinecraftServersFileHandler
    {
        private readonly string _serverPath;
        // this backup dir is only used to temporarily backup the server files while the server is upgrading
        // to a newer version
        private readonly string _backupTempDir;

        private readonly MinecraftServerConfig _serverConfig;

        public MinecraftServersFileHandler(string serversPath, MinecraftServerConfig serverConfig)
        {
            _serverPath = serversPath;
            _backupTempDir = Path.Combine(_serverPath, "backup");
            _serverConfig = serverConfig;
        }



        /// <summary>
        /// Accepts eula for a minecraft server.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="MCInternalException"></exception>
        public async Task AcceptEula()
        {
            string eulaPath = Path.Combine(_serverPath, "eula.txt");
            if (!File.Exists(eulaPath))
                throw new MCInternalException("Couldn't find eula.txt in folder " + _serverPath);


            string eulaText = await File.ReadAllTextAsync(eulaPath);

            if (!eulaText.Contains("eula=false"))
                return;

            eulaText = eulaText.Replace("eula=false", "eula=true");
            await File.WriteAllTextAsync(eulaPath, eulaText);
        }

        /// <summary>
        /// Backs up temporarily the important files for a minecraft server. It moves the files to a backup folder.
        /// </summary>
        public void BackUpImportantFiles()
        {
            if(Directory.Exists(_backupTempDir))
                Directory.Delete(_backupTempDir, true);

            var backupDir = Directory.CreateDirectory(_backupTempDir);

            var info = new DirectoryInfo(_serverPath);

            // https://www.sportskeeda.com/minecraft-wiki/how-to-update-server-minecraft#:~:text=To%20update%20a%20server%20in%20Minecraft%2C%20create%20a%20new%20folder,executable%20into%20the%20old%20folder.
            string[] importantFiles = { "banned-ips.json", "banned-players.json", "ops.json", "server.properties", "usercache.json", "whitelist.json", "server.info" };
            string[] importantFolders = { "world" };

            foreach (var file in importantFiles.Select(f => new FileInfo(Path.Combine(_serverPath, f))).Where(f => f.Exists))
                file.MoveTo(Path.Combine(backupDir.FullName, file.Name));

            foreach (var dir in importantFolders.Select(d => new DirectoryInfo(Path.Combine(_serverPath, d))).Where(d => d.Exists))
                dir.MoveTo(Path.Combine(backupDir.FullName, dir.Name));
        }

        /// <summary>
        /// Removes all the remanaining files from the folder, but it doesn't touch the backup folder.
        /// </summary>
        public void RemoveAllFilesExceptBackupFolder()
        {
            foreach (var file in Directory.GetFiles(_serverPath))
                File.Delete(file);

            foreach (var folder in Directory.GetDirectories(_serverPath).Where(dir => dir != _backupTempDir))
                Directory.Delete(folder, true);
        }

        /// <summary>
        /// Restores the important files from the backup folder to the server folder.
        /// </summary>
        /// <exception cref="MCInternalException"></exception>
        public void RetrieveBackedUpFiles()
        {
            var backupDir = new DirectoryInfo(_backupTempDir);

            if (!backupDir.Exists)
                throw new MCInternalException("Could not find backup directory!");

            foreach (var file in backupDir.GetFiles())
            {
                string originalFilePath = Path.Combine(_serverPath, file.Name);
                if(File.Exists(originalFilePath))
                    File.Delete(originalFilePath);
                file.MoveTo(originalFilePath);
            }
                

            foreach (var dir in backupDir.GetDirectories())
            {
                string originalDirPath = Path.Combine(_serverPath, dir.Name);
                if(Directory.Exists(originalDirPath))
                    Directory.Delete(originalDirPath, true);
                dir.MoveTo(originalDirPath);
            }
        }

        public void DeleteTemporaryBackupFolder()
        {
            Directory.Delete(_backupTempDir, true);
        }


        /// <summary>
        /// Zips the server folder and saves it into the centralized backup folder.
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="name"></param>
        /// <param name="isAutomatic"></param>
        /// <returns></returns>
        public async Task Backup(long serverId, string name, BackupType type)
        {
            var backupManager = BackupManager.Instance;

            // deleting oldest backup if limit is reached
            var backups = await backupManager.GetBackupsByServer(serverId);
            backups = backups.Where(b => b.Type == type);
            var limit = type == BackupType.Automatic ? _serverConfig.MaxAutoBackup : _serverConfig.MaxManualBackup;

            if (backups.Count() >= limit)
            {
                var oldestBackup = backups.OrderBy(b => b.CreationTime).First();
                await backupManager.DeleteBackup(oldestBackup);
            }

            // creating new backup
            string fromDir = _serverPath;
            string backupPath = await BackupManager.Instance.CreateBackupPath(serverId, name, type);

            Predicate<string> filter = s => !s.StartsWith("eula.txt") && !s.StartsWith("logs") && !s.StartsWith("server.info");
            await FileHelper.CreateZipFromDirectory(fromDir, backupPath, System.IO.Compression.CompressionLevel.SmallestSize, false, filter);
        }

        /// <summary>
        /// Restores a backup from the centralized backup folder to the server folder. 
        /// WARNING: THIS WILL OVERWRITE ALL THE FILES IN THE SERVER FOLDER!
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="backup"></param>
        /// <returns></returns>
        /// <exception cref="MCExternalException"></exception>
        public async Task RestoreBackup(long serverId, IBackup backup)
        {
            var backupManager = BackupManager.Instance;
            var backupPath = await backupManager.CreateBackupPath(serverId, backup.Name, backup.Type);

            if (backup == null)
                throw new MCExternalException("Backup not found!");

            if (backup.ServerId != serverId)
                throw new MCExternalException("Backup does not belong to this server!");
            
            // extracting backup
            await FileHelper.ExtractToDirectory(backupPath, _serverPath);
        }
    }
}
