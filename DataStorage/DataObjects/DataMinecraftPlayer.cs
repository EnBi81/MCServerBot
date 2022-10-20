using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.DataObjects
{
    public class DataMinecraftPlayer
    {
        public ulong PlayerId { get; internal init; }
        public string Username { get; internal init; } = null!;
    }
}
