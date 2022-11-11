using Application.Minecraft.MinecraftServers;
using Shared.Exceptions;
using Shared.Model;
using static Shared.Model.ILogMessage;

namespace Application.Minecraft.States
{
    /// <summary>
    /// This state is when the server files are created, or the server is being upgraded to a newer version.
    /// </summary>
    internal class MaintenanceState : ServerStateAbs
    {
        
        public MaintenanceState(MinecraftServerLogic server) : base(server)
        {
        }

        
        /// <summary>
        /// Starts the maintenance routine
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public override Task Start(string username)
        {
            var logMessage = new LogMessage(username + ": " + "Starting Server Maintenance " + _server.ServerName, LogMessageType.System_Message);
            _server.AddLog(logMessage);

            Thread t = new Thread(async () =>
            {
                if (IsServerNew())
                    await CreateServerFiles();
                else
                    await UpgradeServerToNewVersion();

                _server.SetServerState<OfflineState>();
            });
            t.Start();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns if the server is new or not
        /// </summary>
        /// <returns></returns>
        private bool IsServerNew()
        {
            return !new DirectoryInfo(_server.ServerPath).GetFiles().Any();
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

            AddSystemLog("Accepting Eula...");
            await _server.McServerFileHandler.AcceptEula();
            AddSystemLog("Eula Accepted.");

            _server.SetServerState<OfflineState>(true);
        }

        /// <summary>
        /// Upgrades the server to a newer minecraft version.
        /// </summary>
        /// <returns></returns>
        private async Task UpgradeServerToNewVersion()
        {
            AddSystemLog("Upgrading Server to new Version");

            var process = await _server.McServerProcess.Start(_server.MCVersion);
            await process.WaitForExitAsync();

            _server.McServerFileHandler.RemoveUnneccessaryFiles();
            AddSystemLog("Backing up important files...");
            _server.McServerFileHandler.BackUpImportantFiles();
            AddSystemLog("Important files backed up.");

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
        /// Cannot stop anything now.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="MinecraftServerException"></exception>
        public override Task Stop(string username) => throw new MinecraftServerException("Please wait till the maintenance finishes!");
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
