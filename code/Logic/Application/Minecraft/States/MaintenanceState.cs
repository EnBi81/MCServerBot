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
    internal class MaintenanceState : ServerStateAbs
    {

        public MaintenanceState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        
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
        /// Creates the server files from scratch
        /// </summary>
        /// <returns></returns>
        private async Task CreateServerFiles()
        {
            AddSystemLog("Creating Server Files...");

            var process = await _server.StartServerProcess();


            // https://nodecraft.com/support/games/minecraft/minecraft-eula#:~:text=Starting%20with%20Minecraft%20version%201.7,.com%2Fdocuments%2Fminecraft_eula.
            // below 1.8, there is no eula, and the server will start automatically
            if (new Version(_server.MCVersion.Version) < new Version("1.8"))
            {
                async void ShutdownServerWhenReady(object? sender, ILogMessage e) {
                    if(e.Message.Contains("Done"))
                        await _server.McServerProcess.WriteToStandardInputAsync("stop");
                }
                
                _server.LogReceived += ShutdownServerWhenReady;
                await process.WaitForExitAsync();
                _server.LogReceived -= ShutdownServerWhenReady;
            }
            // from 1.8, the server will shut down after creating the files, and we need to accept the eula
            else
            {
                await process.WaitForExitAsync();
                AddSystemLog("Accepting Eula...");
                await _server.McServerFileHandler.AcceptEula();
                AddSystemLog("Eula Accepted.");
            }
        }

        /// <summary>
        /// Upgrades the server to a newer minecraft version.
        /// </summary>
        /// <returns></returns>
        private async Task UpgradeServerToNewVersion()
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

                await SetNewState<OfflineState>();
                throw;
            }
            

            AddSystemLog("Retrieving backed up files...");
            
            // remove the temp trash
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempTrash);
            // move temp backup to server folder
            _server.McServerFileHandler.MoveItems(ServerFolder.TempBackup, ServerFolder.ServerFolder);
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempBackup);
            
            AddSystemLog("Backed up files retrieved.");
        }

        /// <summary>
        /// Adds the log message as a system message.
        /// </summary>
        /// <param name="text"></param>
        private void AddSystemLog(string text)
        {
            var logMessage = new LogMessage(text, LogMessageType.System_Message);
            HandleLog(logMessage);
        }
        

        public override bool IsAllowedNextState(IServerState state)
        {
            return state is OfflineState;
        }

        public override async Task Apply() 
        {
            var logMessage = new LogMessage("Starting Server Maintenance " + _server.ServerName, LogMessageType.System_Message);
            _server.AddLog(logMessage);

            if (IsServerNew())
            {
                if (args.Length == 0 || args[0] is not MinecraftServerCreationPropertiesDto dto)
                    throw new MCExternalException("No creation properties were provided.");
                    
                await CreateServerFiles();
                await _server.Properties.UpdateProperties(dto);
            }
            else
                await UpgradeServerToNewVersion();

            await SetNewState<OfflineState>();
            _server.McServerInfos.Save(_server);
        }

        /// <summary>
        /// Returns <see cref="ServerState.Maintenance"/>
        /// </summary>
        public override ServerStatus Status => ServerStatus.Maintenance;
        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool IsRunning => true;
        /// <summary>
        /// Adds the log message to the log collection.
        /// </summary>
        /// <param name="logMessage"></param>
        public override void HandleLog(LogMessage logMessage) => _server.AddLog(logMessage);
        /// <summary>
        /// Cannot write command during maintenance,
        /// </summary>
        /// <param name="command"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="MinecraftServerException"></exception>
        public override Task WriteCommand(string? command, string username) => throw new MinecraftServerException("You cannot add commands during maintenance!");
        
    }
}
