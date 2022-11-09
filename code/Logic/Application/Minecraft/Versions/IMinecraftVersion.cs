using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Versions
{
    public interface IMinecraftVersion
    {
        string Name { get; }
        string Version { get; }
        DateTime FullReleaseDate { get; }
        
        string DownloadUrl { get; }
        bool IsDownloaded { get; }
        string? AbsoluteJarPath { get; }
        
        void Download();
    }
}
