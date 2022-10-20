using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Implementations.SQLite
{
    internal class MinecraftEventRegister : IMinecraftEventRegister
    {
        public void AddMeasurement(ulong serverId, double cpu, long memory)
        {
            throw new NotImplementedException();
        }

        public void PlayerJoined(ulong serverId, string username)
        {
            throw new NotImplementedException();
        }

        public void PlayerLeft(ulong serverId, string username)
        {
            throw new NotImplementedException();
        }

        public void SetDiskSize(ulong serverId, long diskSize)
        {
            throw new NotImplementedException();
        }
    }
}
