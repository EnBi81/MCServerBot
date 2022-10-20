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
        Task RegisterDiscordUser(ulong discordId, string username, string profilepic, string webAccessToken);
        Task GrantPermission(ulong userId, ulong discordId);
        Task RevokePermission(ulong userId, ulong discordId);
        Task<bool> HasPermission(ulong id);
        Task<DataUser?> GetUser(ulong id);
        Task RefreshUser(ulong id, string username, string profilePicUrl);
    }
}
