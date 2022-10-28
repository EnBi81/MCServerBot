using Shared.DTOs;

namespace Application.DAOs.Database
{
    public interface IPermissionDataAccess
    {
        Task RegisterDiscordUser(ulong discordId, string username, string profilepic, string webAccessToken);
        Task GrantPermission(ulong userId, ulong discordId);
        Task RevokePermission(ulong userId, ulong discordId);
        Task<string?> GetWebAccessCode(ulong id);
        Task<bool> HasPermission(ulong id);
        Task<bool> HasPermission(string token);
        Task<DataUser?> GetUser(ulong id);
        Task<DataUser?> GetUser(string token);
        Task RefreshUser(ulong id, string username, string profilePicUrl);
    }
}
