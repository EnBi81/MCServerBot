using DataStorage.DataObjects;

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
        Task<string?> GetServerName(ulong serverId);
    }
}
