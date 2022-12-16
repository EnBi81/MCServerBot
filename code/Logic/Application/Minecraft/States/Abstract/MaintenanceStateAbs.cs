using Application.Minecraft.MinecraftServers;
using Application.Minecraft.Versions;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States.Abstract
{
    internal abstract class MaintenanceStateAbs : ServerStateAbs
    {
        protected MaintenanceStateAbs(MinecraftServerLogic server, object[] args) : base(server, args) { }

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
        public override void HandleLog(LogMessage logMessage) { }
        /// <summary>
        /// Cannot write command during maintenance,
        /// </summary>
        /// <param name="command"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="MinecraftServerException"></exception>
        public override Task WriteCommand(string? command, string username) 
            => throw new MCExternalException("You cannot add commands during maintenance!");
        /// <summary>
        /// Only offline state is allowed
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public override bool IsAllowedNextState(IServerState state) => state is OfflineState;


        /// <summary>
        /// Creates the server files from scratch
        /// </summary>
        /// <returns></returns>
        protected async Task CreateServerFiles(IMinecraftVersion? version = null)
        {
            AddSystemLog("Creating Server Files...");

            var process = version is null ?
                await _server.StartServerProcess() :
                await _server.McServerProcess.Start(version);


            // https://nodecraft.com/support/games/minecraft/minecraft-eula#:~:text=Starting%20with%20Minecraft%20version%201.7,.com%2Fdocuments%2Fminecraft_eula.
            // below 1.8, there is no eula, and the server will start automatically
            if (new Version(_server.MCVersion.Version) < new Version("1.8"))
            {
                async void ShutdownServerWhenReady(object? sender, ILogMessage e)
                {
                    if (e.Message.Contains("Done"))
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
        /// Adds the log message as a system message.
        /// </summary>
        /// <param name="text"></param>
        protected void AddSystemLog(string text)
        {
            var logMessage = new LogMessage(text, LogMessageType.System_Message);
            _server.AddLog(logMessage);
        }
    }
}
