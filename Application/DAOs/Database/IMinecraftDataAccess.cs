using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAOs.Database
{
    /// <summary>
    /// Database access for a minecraft server.
    /// </summary>
    public interface IMinecraftDataAccess
    {
        /// <summary>
        /// Adds the measurement to the database.
        /// </summary>
        /// <param name="serverId">Server which got measured.</param>
        /// <param name="cpu">cpu percentage measured.</param>
        /// <param name="memory">memory bytes measured.</param>
        void AddMeasurement(long serverId, double cpu, long memory);
        /// <summary>
        /// Sets the disk size for a minecraft server.
        /// </summary>
        /// <param name="serverId">id of the server.</param>
        /// <param name="diskSize">disk size in bytes of the server.</param>
        void SetDiskSize(long serverId, long diskSize);
        /// <summary>
        /// Registers the event a player joined to the minecraft server.
        /// </summary>
        /// <param name="serverId">server id the player joined in.</param>
        /// <param name="username">player's username.</param>
        void PlayerJoined(long serverId, string username);
        /// <summary>
        /// Registers the event a player left a minecraft server. 
        /// </summary>
        /// <param name="serverId">server id the player left.</param>
        /// <param name="username">player's username.</param>
        void PlayerLeft(long serverId, string username);
        /// <summary>
        /// Registers a command written by a user.
        /// </summary>
        /// <param name="serverId">id of the server the command was written</param>
        /// <param name="command">the command</param>
        /// <param name="userEventData">user event data</param>
        void WriteCommand(long serverId, string command, UserEventData userEventData);
    }
}
