using Prismarine.NET.Model;

namespace Prismarine.NET.Networking.Interfaces
{
    internal interface IServerParkService
    {
        Task<IEnumerable<IMinecraftServer>> GetAllServers();
    }
}
