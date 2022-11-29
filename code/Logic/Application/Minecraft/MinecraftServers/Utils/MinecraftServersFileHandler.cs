using Shared.Exceptions;

namespace Application.Minecraft.MinecraftServers.Utils
{
    internal class MinecraftServersFileHandler
    {
        private readonly string _serverPath;
        // this backup dir is only used to temporarily backup the server files while the server is upgrading
        // to a newer version
        private readonly string _backupTempDir;

        private readonly string _backupFolder;

        public MinecraftServersFileHandler(string serversPath)
        {
            _serverPath = serversPath;
            _backupTempDir = Path.Combine(_serverPath, "backup");
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

        
        public void RemoveAllFilesExceptBackupFolder()
        {
            foreach (var file in Directory.GetFiles(_serverPath))
                File.Delete(file);

            foreach (var folder in Directory.GetDirectories(_serverPath).Where(dir => dir != _backupTempDir))
                Directory.Delete(folder, true);
        }
        
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
    }
}
