using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Model
{
    public interface IServerPark
    {
        Task<IEnumerable<IMinecraftServer>> GetAllServers();
    }
}
