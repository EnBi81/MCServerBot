using Prismarine.NET.DTOs;
using Prismarine.NET.Networking.Interfaces;

namespace Prismarine.NET.Model.Implementations
{
    internal class ServerPark : IServerPark
    {
        private readonly IServerParkService _serverParkService;
        private readonly IMinecraftServerService _minecraftServerService;

        public ServerPark(IServerParkService service, IMinecraftServerService minecraftServerService)
        {
            _serverParkService = service;
            _minecraftServerService = minecraftServerService;
        }

        public Task<IMinecraftServer> CreateServerAsync(ServerCreationDto serverInfo) 
        {
            return _serverParkService.CreateServer(serverInfo);
        }

        public Task<IEnumerable<IMinecraftServer>> GetAllServersAsync()
        {
            return _serverParkService.GetAllServers();
        }

        public Task<ICollection<MinecraftVersion>> GetMinecraftVersionsAsync()
        {
             return _serverParkService.GetMinecraftVersions();
        }

        public Task<IMinecraftServer> GetServer(long id) 
        {
            return _minecraftServerService.GetServerById(id);
        }
    }
}
