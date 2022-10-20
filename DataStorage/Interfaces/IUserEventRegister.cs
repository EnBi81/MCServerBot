using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IUserEventRegister
    {
        void NameChange(ulong serverId, string newName);
        void CreateServer(string name);
        void DeleteServer(ulong serverId);
        void WriteCommand(ulong serverId, string command);
        void TurnOnServer(ulong serverId);
        void TurnOffServer(ulong serverId);
    }
}
