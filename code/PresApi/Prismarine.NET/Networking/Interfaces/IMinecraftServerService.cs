using Prismarine.NET.Model;

namespace Prismarine.NET.Networking.Interfaces
{
    internal interface IMinecraftServerService
    {
        /// <summary>
        /// Gets a server by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IMinecraftServer> GetServerById(long id);

        /// <summary>
        /// Gets the server icon path
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetServerIconPath(long id);
    }
}
