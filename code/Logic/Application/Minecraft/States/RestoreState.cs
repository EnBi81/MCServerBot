using Application.Minecraft.MinecraftServers;
using Application.Minecraft.MinecraftServers.Utils;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States
{
    [ManualState]
    internal class RestoreState : ServerStateAbs
    {
        public RestoreState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        public override ServerStatus Status => ServerStatus.Restore;

        public override bool IsRunning => false;

        public override async Task Apply()
        {
            if (args is { Length: 0} || args[0] is not IBackup backup)
            {
                await SetNewStateAsync<OfflineState>();
                throw new MCInternalException("No backup present when restoring");
            }

            AddSystemLog($"Restoring backup {backup.ServerId}-{backup.Name} ...");

            string[] itemsToBackup = new[] { "eula.txt" };


            // empty temp folders
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempBackup);
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempTrash);
            // move important files to backup
            _server.McServerFileHandler.MoveItems(ServerFolder.ServerFolder, ServerFolder.TempBackup, itemsToBackup);
            // move other files to trash
            _server.McServerFileHandler.MoveItems(ServerFolder.ServerFolder, ServerFolder.TempTrash);

            try
            {
                await _server.McServerFileHandler.RestoreBackup(_server.Id, backup);
            }
            catch
            {
                AddSystemLog($"Failed to restore backup {backup.ServerId}-{backup.Name}. Rolling back...");
                _server.McServerFileHandler.EmptyFolder(ServerFolder.ServerFolder);
                _server.McServerFileHandler.MoveItems(ServerFolder.TempBackup, ServerFolder.ServerFolder);
                _server.McServerFileHandler.MoveItems(ServerFolder.TempTrash, ServerFolder.ServerFolder);

                await SetNewStateAsync<OfflineState>();
                throw;
            }

            AddSystemLog($"Restore {backup.ServerId}-{backup.Name} done! Finishing post processing...");

            // remove the temp trash
            _server.McServerFileHandler.EmptyFolder(ServerFolder.TempTrash);
            // move temp backup to server folder
            _server.McServerFileHandler.MoveItems(ServerFolder.TempBackup, ServerFolder.ServerFolder);


            await SetNewStateAsync<OfflineState>();
        }


        /// <summary>
        /// Adds the log message as a system message.
        /// </summary>
        /// <param name="text"></param>
        private void AddSystemLog(string text)
        {
            var logMessage = new LogMessage(text, LogMessageType.System_Message);
            _server.AddLog(logMessage);
        }

        public override void HandleLog(LogMessage logMessage) { }
        public override bool IsAllowedNextState(IServerState state) => state is OfflineState;
        public override Task WriteCommand(string? command, string username) => throw new MinecraftServerException(_server.ServerName + " is restoring!");
    }
}
