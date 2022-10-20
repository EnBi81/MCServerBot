using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IServerParkEventRegister
    {
        Task<ulong> GetMaxServerId();
        Task CreateServer(ulong userId, ulong serverId, string serverName);
        Task DeleteServer(ulong userId, ulong serverId);
        Task RenameServer(ulong userId, ulong serverId, string newName);
        Task StartServer(ulong userId, ulong serverId);
        Task StopServer(ulong userId, ulong serverId);
    }
}
