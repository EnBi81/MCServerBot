using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Shared.Exceptions;
using Shared.Model;

namespace Application.Minecraft.States
{
    /// <summary>
    /// This state is when the server files are created, or the server is being upgraded to a newer version.
    /// </summary>
    internal class MaintenanceState : ServerStateAbs
    {
        private bool _isRunning = false;


        public MaintenanceState(MinecraftServerLogic server, string[] args) : base(server, args) { }

        
        /// <summary>
        /// Returns if the server is new or not
        /// </summary>
        /// <returns></returns>
        private bool IsServerNew()
        {
            return !new DirectoryInfo(_server.ServerPath).GetFiles().Any(f => f.Name != "server.info");
        }

        /// <summary>
        /// Creates the server files from scratch
        /// </summary>
        /// <returns></returns>
        private async Task CreateServerFiles()
        {
            AddSystemLog("Creating Server Files...");

            var process = await _server.StartServerProcess();
            await process.WaitForExitAsync();

            await _server.Properties.UpdateProperties(_server.CreationProperties);

            AddSystemLog("Accepting Eula...");
            await _server.McServerFileHandler.AcceptEula();
            AddSystemLog("Eula Accepted.");

            
        }

        /// <summary>
        /// Upgrades the server to a newer minecraft version.
        /// </summary>
        /// <returns></returns>
        private async Task UpgradeServerToNewVersion()
        {
            AddSystemLog("Upgrading Server to new Version");
            
            AddSystemLog("Backing up important files...");
            _server.McServerFileHandler.BackUpImportantFiles();
            AddSystemLog("Important files backed up.");
            _server.McServerFileHandler.RemoveAllFilesExceptBackupFolder();

            await CreateServerFiles();

            AddSystemLog("Retrieving backed up files...");
            _server.McServerFileHandler.RetrieveBackedUpFiles();
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
            if (state is OfflineState)
                return true;

            if (state is BackupAutoState)
                return false;

            throw new MinecraftServerException(_server.ServerName + " is in Maintenance. Please wait!");
        }

        public override async Task Apply() 
        {
            _isRunning = true;
            var logMessage = new LogMessage("Starting Server Maintenance " + _server.ServerName, LogMessageType.System_Message);
            _server.AddLog(logMessage);
            
            if (IsServerNew())
                await CreateServerFiles();
            else
                await UpgradeServerToNewVersion();

            await _server.SetServerStateAsync<OfflineState>();
            _server.McServerInfos.Save(_server);
        }

        /// <summary>
        /// Returns <see cref="ServerState.Maintenance"/>
        /// </summary>
        public override ServerStatus Status => ServerStatus.Maintenance;
        /// <summary>
        /// Returns true.
        /// </summary>
        public override bool IsRunning => _isRunning;
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
