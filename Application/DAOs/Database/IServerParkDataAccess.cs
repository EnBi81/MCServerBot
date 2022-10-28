using Shared.DTOs;

namespace Application.DAOs.Database
{
    /// <summary>
    /// Responsible for handling the ServerPark related data.
    /// </summary>
    public interface IServerParkDataAccess
    {
        /// <summary>
        /// Gets the largest id of the registered servers.
        /// </summary>
        /// <returns>the largest id of the registered servers.returns>
        Task<ulong> GetMaxServerId();
        /// <summary>
        /// Registers a new server in the database.
        /// </summary>
        /// <param name="serverId">the id of the server.</param>
        /// <param name="serverName">the server's name.</param>
        /// <param name="userEventData">user event data object.</param>
        /// <returns>A task representing the operation.</returns>
        Task CreateServer(ulong serverId, string serverName, UserEventData userEventData);
        /// <summary>
        /// Sets an existing server as deleted in the database.
        /// </summary>
        /// <param name="serverId">id of the server to set to deleted.</param>
        /// <param name="userEventData">user event data object.</param>
        /// <returns>A task representing the operation.</returns>
        Task DeleteServer(ulong serverId, UserEventData userEventData);
        /// <summary>
        /// Renames a server.
        /// </summary>
        /// <param name="serverId">the id of the server.</param>
        /// <param name="newName"></param>
        /// <param name="userEventData">user event data object.</param>
        /// <returns>A task representing the operation.</returns>
        Task RenameServer(ulong serverId, string? newName, UserEventData userEventData);
        /// <summary>
        /// Registers a start event.
        /// </summary>
        /// <param name="serverId">the id of the server.</param>
        /// <param name="userEventData">user event data object.</param>
        /// <returns>A task representing the operation.</returns>
        Task StartServer(ulong serverId, UserEventData userEventData);
        /// <summary>
        /// Registers a stop event.
        /// </summary>
        /// <param name="serverId">the id of the server.</param>
        /// <param name="userEventData">user event data object.</param>
        /// <returns>A task representing the operation.</returns>
        Task StopServer(ulong serverId, UserEventData userEventData);
        /// <summary>
        /// Gets a server's name.
        /// </summary>
        /// <param name="serverId">the id of the server.</param>
        /// <returns>A task representing the operation.</returns>
        Task<string?> GetServerName(ulong serverId);
    }
}
