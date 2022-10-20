using DataStorage.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IDiscordEventRegister
    {
        Task CreateServer(ulong userId, string serverName, long storageSpace);
        Task DeleteServer(ulong userId, ulong serverId);
        Task RenameServer(ulong userId, ulong serverId, string newName);

        Task GrantPermission(ulong userId, ulong discordId, string username, string profilepic, string webAccessToken);
        Task RevokePermission(ulong userId, ulong discordId);
        Task<bool> HasPermission(ulong id);
        Task<DataUser> GetUser(ulong id);
        Task RefreshUser(ulong id, string username, string profilePicUrl);

        Task StartServer(ulong userId, ulong serverId);
        Task StopServer(ulong userId, ulong serverId);
    }
}
