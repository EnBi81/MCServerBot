using Application.Minecraft.Backup;
using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using Microsoft.VisualBasic.FileIO;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.States
{
    [ManualState]
    internal class DeleteState : ServerStateAbs
    {
        public DeleteState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        public override async Task Apply() 
        {
            await BackupManager.Instance.DeleteServerBackupsAsync(_server.Id);

            var serverDir = new DirectoryInfo(_server.FileStructure.RootFolder);
            FileSystem.DeleteDirectory(serverDir.FullName, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);

            _server.AddLog(new LogMessage("Server deleted", LogMessageType.System_Message));
        }

        public override ServerStatus Status => ServerStatus.Deleting;
        public override bool IsRunning => false;
        public override void HandleLog(LogMessage logMessage) { }
        public override bool IsAllowedNextState(IServerState state) => false;
    }
}
