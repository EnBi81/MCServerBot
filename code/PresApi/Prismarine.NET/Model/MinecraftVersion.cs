using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.Model
{
    public class MinecraftVersion
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string DownloadUrl { get; set; }

        public bool IsDownloaded { get; set; }
    }
}
