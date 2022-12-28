using Prismarine.NET.Networking.Interfaces;

namespace Prismarine.NET.Model.Implementations
{
    internal class ServerPark : IServerPark
    {
        private readonly IServerParkService _service;
        
        public ServerPark(IServerParkService service)
        {
            _service = service;
        }

        public Task<IEnumerable<IMinecraftServer>> GetAllServers()
        {
            return _service.GetAllServers();
        }
    }
}
