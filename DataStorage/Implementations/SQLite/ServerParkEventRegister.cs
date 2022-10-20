using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Implementations.SQLite
{
    internal class ServerParkEventRegister : IServerParkEventRegister
    {
        public Task CreateServer(ulong userId, ulong serverId, string serverName)
        {
            throw new NotImplementedException();
        }

        public Task DeleteServer(ulong userId, ulong serverId)
        {
            throw new NotImplementedException();
        }

        public Task<ulong> GetMaxServerId()
        {
            throw new NotImplementedException();
        }

        public Task RenameServer(ulong userId, ulong serverId, string newName)
        {
            throw new NotImplementedException();
        }

        public Task StartServer(ulong userId, ulong serverId)
        {
            throw new NotImplementedException();
        }

        public Task StopServer(ulong userId, ulong serverId)
        {
            throw new NotImplementedException();
        }
    }
}
