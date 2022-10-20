using DataStorage.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IWebsiteEventRegister
    {
        Task DeleteServer(string token, ulong serverId);
        Task AddServer(string token, ulong serverId, string newName);

        Task StartServer(string token, ulong serverId);
        Task StopServer(string token, ulong serverId);
        Task AddCommand(string token, string command);

        Task<bool> HasPermission(string token);
    }
}
