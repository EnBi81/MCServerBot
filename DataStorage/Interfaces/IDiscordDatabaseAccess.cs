using DataStorage.DataObjects;

namespace DataStorage.Interfaces
{
    /// <summary>
    /// Database access for discord.
    /// </summary>
    public interface IDiscordDatabaseAccess
    {
        Task RegisterDiscordUser(ulong discordId, string username, string profilepic, string webAccessToken);
        Task GrantPermission(ulong userId, ulong discordId);
        Task RevokePermission(ulong userId, ulong discordId);
        Task<bool> HasPermission(ulong id);
        Task<DataUser?> GetUser(ulong id);
        Task RefreshUser(ulong id, string username, string profilePicUrl);
    }
}
