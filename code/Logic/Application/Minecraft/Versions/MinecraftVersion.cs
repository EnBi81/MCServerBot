using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Versions
{
    internal class MinecraftVersion : IMinecraftVersion
    {
        public string Name { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string FullRelease { get; set; } = null!;
        public DateTime FullReleaseDate => DateTime.Parse(FullRelease);
        public string DownloadUrl { get; set; } = null!;

        public bool IsDownloaded => throw new NotImplementedException();

        public string? AbsoluteJarPath => throw new NotImplementedException();

        public MinecraftVersion() { }

        public void Download() => throw new NotImplementedException();
    }
}
