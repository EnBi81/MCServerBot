using Prismarine.NET.Model.Enums;
using Prismarine.NET.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Model.Implementations
{
    internal class MinecraftServer : IMinecraftServer
    {
        private readonly IMinecraftServerService _minecraftServerService;

        public MinecraftServer(IMinecraftServerService minecraftServerService) {
            _minecraftServerService = minecraftServerService;
        }


        public long Id { get; init; }

        public string ServerName { get; set; }
        
        public ServerStatus Status { get; set; }

        public bool IsRunning { get; set; }

        public ICollection<LogMessage> Logs { get; init; }

        public DateTime? OnlineFrom { get; set; }

        public long StorageBytes { get; set; }

        public MinecraftVersion MCVersion { get; set; }
        
        public string ServerIcon => throw new NotImplementedException();


        public Task Delete() => throw new NotImplementedException();
        public Task Modify() => throw new NotImplementedException();
        public Task Refresh() => throw new NotImplementedException();
        public Task Toggle() => throw new NotImplementedException();
        public Task WriteCommand() => throw new NotImplementedException();
    }
}
