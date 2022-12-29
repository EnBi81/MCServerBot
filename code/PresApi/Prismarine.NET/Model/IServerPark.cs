using Prismarine.NET.DTOs;

namespace Prismarine.NET.Model
{
    public interface IServerPark
    {
        /// <summary>
        /// Gets all the servers
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IMinecraftServer>> GetAllServersAsync();
        /// <summary>
        /// Creates a new server
        /// </summary>
        /// <returns></returns>
        Task<IMinecraftServer> CreateServerAsync(ServerCreationDto serverInfo);

        /// <summary>
        /// Gets the available minecraft versions
        /// </summary>
        /// <returns></returns>
        Task<ICollection<MinecraftVersion>> GetMinecraftVersionsAsync();
        /// <summary>
        /// Gets the server with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IMinecraftServer> GetServer(long id);
    }
}
