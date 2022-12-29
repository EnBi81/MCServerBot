using Prismarine.NET.Model;
using Prismarine.NET.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Networking.JsonData
{
    internal class MinecraftServerJson
    {
        public long Id { get; init; }

        public string? ServerName { get; set; }

        public ServerStatus Status { get; set; }

        public bool IsRunning { get; set; }

        public ICollection<LogMessage>? Logs { get; init; }

        public DateTime? OnlineFrom { get; set; }

        public long StorageBytes { get; set; }

        public MinecraftVersion? MCVersion { get; set; }
    }
}
