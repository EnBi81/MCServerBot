using DataStorage.DataObjects;
using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Implementations.SQLite
{
    internal class DiscordEventRegister : IDiscordEventRegister
    {
        public Task<DataUser?> GetUser(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task GrantPermission(ulong userId, ulong discordId, string username, string profilepic, string webAccessToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPermission(ulong id)
        {
            throw new NotImplementedException();
        }

        public Task RefreshUser(ulong id, string username, string profilePicUrl)
        {
            throw new NotImplementedException();
        }

        public Task RevokePermission(ulong userId, ulong discordId)
        {
            throw new NotImplementedException();
        }
    }
}
