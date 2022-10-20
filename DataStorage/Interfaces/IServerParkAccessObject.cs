using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Interfaces
{
    public interface IServerParkAccessObject
    {
        string GetName(ulong id);
        ulong CreateMCServer(long storage);
    }
}
