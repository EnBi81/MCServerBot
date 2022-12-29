using Prismarine.NET.DTOs;
using Prismarine.NET.Model;

namespace Prismarine.NET.Networking.Interfaces
{
    internal interface IServerParkService
    {
        /// <summary>
        /// Gets all the servers.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IMinecraftServer>> GetAllServers();
        /// <summary>
        /// Creates a new server.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<IMinecraftServer> CreateServer(ServerCreationDto dto);
        /// <summary>
        /// Gets the available minecraft versions.
        /// </summary>
        /// <returns></returns>
        Task<ICollection<MinecraftVersion>> GetMinecraftVersions();
    }
}
