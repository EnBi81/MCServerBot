using DataStorage.DataObjects;
using DataStorage.DataObjects.Enums;
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
        Task CreateServer(ulong serverId, string serverName, UserEventData userEventData);
        Task DeleteServer(ulong serverId, UserEventData userEventData);
        Task RenameServer(ulong serverId, string? newName, UserEventData userEventData);
        Task StartServer(ulong serverId, UserEventData userEventData);
        Task StopServer(ulong serverId, UserEventData userEventData);
    }
}
