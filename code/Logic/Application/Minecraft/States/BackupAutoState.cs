using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Shared.Exceptions;
using Shared.Model;

namespace Application.Minecraft.States
{
    /// <summary>
    /// This state handles the backing up
    /// </summary>
    internal class BackupAutoState : ServerStateAbs
    {
        public BackupAutoState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        public override async Task Apply()
        {
            Console.WriteLine("Backup");
            
            Console.WriteLine("Backup done");
            await _server.SetServerStateAsync<OfflineState>();
        }



        public override ServerStatus Status => ServerStatus.BackUp;

        public override bool IsRunning => false;

        public override bool IsAllowedNextState(IServerState state)
        {
            if(state is not OfflineState)
                throw new MinecraftServerException(_server.ServerName + " is backing up. Please wait!");

            return true;
        }

        public override void HandleLog(LogMessage logMessage) { }

        public override Task WriteCommand(string? command, string username) => 
            throw new MinecraftServerException(_server.ServerName + " is backing up!");
       
    }
}
