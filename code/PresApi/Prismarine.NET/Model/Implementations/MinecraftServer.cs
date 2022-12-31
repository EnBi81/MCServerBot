using Prismarine.NET.Model.Enums;
using Prismarine.NET.Networking.Interfaces;
using Prismarine.NET.Networking.JsonData;

namespace Prismarine.NET.Model.Implementations
{
    internal class MinecraftServer : IMinecraftServer
    {
        private readonly IMinecraftServerService _minecraftServerService;
        
        internal MinecraftServerJson? MinecraftServerJson { get; set; }

        public MinecraftServer(IMinecraftServerService minecraftServerService) {
            _minecraftServerService = minecraftServerService;
        }
        

        public long Id => MinecraftServerJson?.Id ?? throw new Exception("MinecraftServerJson is null");

        public string ServerName => MinecraftServerJson?.ServerName ?? throw new Exception("MinecraftServerJson is null");

        public ServerStatus Status => MinecraftServerJson?.Status ?? throw new Exception("MinecraftServerJson is null");

        public bool IsRunning => MinecraftServerJson?.IsRunning ?? throw new Exception("MinecraftServerJson is null");

        public ICollection<LogMessage> Logs => MinecraftServerJson?.Logs ?? throw new Exception("MinecraftServerJson is null");

        public DateTime? OnlineFrom => MinecraftServerJson?.OnlineFrom;

        public long StorageBytes => MinecraftServerJson?.StorageBytes ?? throw new Exception("MinecraftServerJson is null");

        public MinecraftVersion MCVersion => MinecraftServerJson?.MCVersion ?? throw new Exception("MinecraftServerJson is null");

        public string ServerIcon => throw new NotImplementedException();


        public Task DeleteAsync() => throw new NotImplementedException();
        public Task ModifyAsync() => throw new NotImplementedException();
        public Task RefreshAsync() => throw new NotImplementedException();
        public Task ToggleAsync() => throw new NotImplementedException();
        public Task WriteCommandAsync() => throw new NotImplementedException();
    }
}
