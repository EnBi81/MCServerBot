using Shared.Exceptions;

namespace Application.Minecraft.MinecraftServers.Utils
{
    internal class MinecraftServersFileHandler
    {
        private readonly string _serverPath;
        private readonly string _backupDir;

        public MinecraftServersFileHandler(string serversPath)
        {
            _serverPath = serversPath;
            _backupDir = Path.Combine(_serverPath, "backup");
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
            if(Directory.Exists(_backupDir))
                Directory.Delete(_backupDir, true);

            var backupDir = Directory.CreateDirectory(_backupDir);

            var info = new DirectoryInfo(_serverPath);

            foreach (var file in info.GetFiles())
                file.MoveTo(Path.Combine(backupDir.FullName, file.Name));

            foreach (var dir in info.GetDirectories().Where(dir => dir.FullName != backupDir.FullName))
                dir.MoveTo(Path.Combine(backupDir.FullName, dir.Name));
        }

        public void RemoveUnneccessaryFiles()
        {
            string eula = Path.Combine(_serverPath, "eula.txt");
            File.Delete(eula);
            string logsDir = Path.Combine(_serverPath, "logs");
            Directory.Delete(logsDir, true);
        }
        
        public void RetrieveBackedUpFiles()
        {
            var backupDir = new DirectoryInfo(_backupDir);

            if (!backupDir.Exists)
                throw new MCInternalException("Could not find backup directory!");

            foreach (var file in backupDir.GetFiles())
            {
                string originalFilePath = Path.Combine(_serverPath, file.Name);
                File.Delete(originalFilePath);
                file.MoveTo(originalFilePath);
            }
                

            foreach (var dir in backupDir.GetDirectories())
            {
                string originalDirPath = Path.Combine(_serverPath, dir.Name);
                Directory.Delete(originalDirPath, true);
                dir.MoveTo(originalDirPath);
            }
        }
    }
}
