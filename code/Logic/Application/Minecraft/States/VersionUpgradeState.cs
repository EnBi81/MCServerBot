using Application.Minecraft.MinecraftServers;
using Application.Minecraft.MinecraftServers.Utils;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States
{
    /// <summary>
    /// This state is when the server files are created, or the server is being upgraded to a newer version.
    /// </summary>
    [ManualState]
    internal class VersionUpgradeState : MaintenanceStateAbs
    {

        public VersionUpgradeState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        
        /// <summary>
        /// Returns if the server is new or not
        /// </summary>
        /// <returns></returns>
        private bool IsServerNew()
        {
            var dirInfo = new DirectoryInfo(_server.FileStructure.ServerFiles);
            return dirInfo.GetFiles().Length == 0;
        }

        

        /// <summary>
        /// Upgrades the server to a newer minecraft version.
        /// </summary>
        /// <returns></returns>
        private async Task Apply()
        {
            AddSystemLog("Upgrading Server to new Version");
            
            AddSystemLog("Backing up important files...");

            string[] itemsToBackup = { "world", "banned-ips.",
                "banned-players.", "ops.", "server.properties", "usercache.json", "whitelist.", "white-list" };

            // empty temp folders
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempBackup);
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempTrash);
            // move important files to backup
            _server.McServerFileHandler.MoveItems(ServerFolder.ServerFolder, ServerFolder.TempBackup, itemsToBackup);
            // move other files to trash
            _server.McServerFileHandler.MoveItems(ServerFolder.ServerFolder, ServerFolder.TempTrash);

            AddSystemLog("Important files backed up.");

            try
            {
                await CreateServerFiles();
            }
            catch (Exception e)
            {
                AddSystemLog("Failed to create server files. Rolling back...");
                _server.McServerFileHandler.EmptyFolder(ServerFolder.ServerFolder);
                _server.McServerFileHandler.MoveItems(ServerFolder.TempBackup, ServerFolder.ServerFolder);
                _server.McServerFileHandler.MoveItems(ServerFolder.TempTrash, ServerFolder.ServerFolder);

                await SetNewStateAsync<OfflineState>();
                throw new MCInternalException(e);
            }
            

            AddSystemLog("Retrieving backed up files...");
            
            // remove the temp trash
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempTrash);
            // move temp backup to server folder
            _server.McServerFileHandler.MoveItems(ServerFolder.TempBackup, ServerFolder.ServerFolder);
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempBackup);
            
            AddSystemLog("Backed up files retrieved.");

            await SetNewStateAsync<OfflineState>();
            _server.McServerInfos.Save(_server);
        }
    }
}
