using SharedPublic.DTOs;

namespace Application.DAOs.Database
{
    /// <summary>
    /// Responsible for handling the permission related data.
    /// </summary>
    public interface IPermissionDataAccess
    {
        /// <summary>
        /// Registers a discord user to the database.
        /// </summary>
        /// <param name="discordId">id of the user.</param>
        /// <param name="username">discord username of the user.</param>
        /// <param name="profilepic">discord profile pic url for the user.</param>
        /// <param name="webAccessToken">web access token for the user.</param>
        /// <returns></returns>
        Task RegisterDiscordUser(ulong discordId, string username, string profilepic, string webAccessToken);
        /// <summary>
        /// Grants permission to an already existing user.
        /// </summary>
        /// <param name="userId">discord id of the user who grants permission.</param>
        /// <param name="discordId">discord id of the user who has been granted an access.</param>
        /// <returns></returns>
        Task GrantPermission(ulong userId, ulong discordId);
        /// <summary>
        /// Revokes permission from an already existing user.
        /// </summary>
        /// <param name="userId">discord id of the user who revokes permission.</param>
        /// <param name="discordId">discord id of the user who has been revoked the access.</param>
        /// <returns></returns>
        Task RevokePermission(ulong userId, ulong discordId);
        /// <summary>
        /// Gets the web access code for a user.
        /// </summary>
        /// <param name="id">discord id for the user.</param>
        /// <returns></returns>
        Task<string?> GetWebAccessCode(ulong id);
        /// <summary>
        /// Gets if a user has permission to the system.
        /// </summary>
        /// <param name="id">discord id of the user</param>
        /// <returns>true if the user has permission; else false.</returns>
        Task<bool> HasPermission(ulong id);
        /// <summary>
        /// Gets if a token has permission to the system.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>true if the user has permission; else false.</returns>
        Task<bool> HasPermission(string token);
        /// <summary>
        /// Gets the user by discord id.
        /// </summary>
        /// <param name="id">discord id</param>
        /// <returns>the DataUser object if the id is registered; else null</returns>
        Task<DataUser?> GetUser(ulong id);
        /// <summary>
        /// Gets the user by token.
        /// </summary>
        /// <param name="token">the web access token.</param>
        /// <returns>the DataUser object if the token is registered; else null</returns>
        Task<DataUser?> GetUser(string token);
        /// <summary>
        /// Refreshes a user's data
        /// </summary>
        /// <param name="id">discord id of the user to refresh.</param>
        /// <param name="username">new username</param>
        /// <param name="profilePicUrl">new profile pic</param>
        /// <returns></returns>
        Task RefreshUser(ulong id, string username, string profilePicUrl);
    }
}
