using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IMinecraftEventRegister
    {
        void AddMeasurement(ulong serverId, double cpu, long memory);
        void SetDiskSize(ulong serverId, long diskSize);
        void PlayerJoined(ulong serverId, string username);
        void PlayerLeft(ulong serverId, string username);
    }
}
