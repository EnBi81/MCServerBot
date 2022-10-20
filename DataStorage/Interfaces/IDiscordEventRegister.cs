using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IDiscordEventRegister
    {
        void CreateServer(ulong userId, string serverName, long storageSpace);
        void DeleteServer(ulong userId, ulong serverId);
        void RenameServer(ulong userId, ulong serverId, string newName);

        void GrantPermission(ulong userId, ulong discordId, string username, string profilepic, string webAccessToken);
        void RevokePermission(ulong userId, ulong discordId);
        
        void StartServer(ulong userId, ulong serverId);
        void StopServer(ulong userId, ulong serverId);
    }
}
