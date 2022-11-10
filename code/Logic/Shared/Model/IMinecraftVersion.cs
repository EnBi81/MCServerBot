using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Versions
{
    /// <summary>
    /// Holds information about a minecraft version
    /// </summary>
    public interface IMinecraftVersion
    {
        /// <summary>
        /// Name of the version
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Version (etc. 1.19.2)
        /// </summary>
        string Version { get; }
        /// <summary>
        /// Release date
        /// </summary>
        DateTime ReleaseDate { get; }
        /// <summary>
        /// Download url for the server.jar
        /// </summary>
        string DownloadUrl { get; }
        /// <summary>
        /// Gets if the version is downloaded locally
        /// </summary>
        bool IsDownloaded { get; }
        /// <summary>
        /// Gets the absolute path of the server.jar if the version is downloaded locally. Else null.
        /// </summary>
        string? AbsoluteJarPath { get; }
        
        /// <summary>
        /// Downloads the version
        /// </summary>
        void Download();

        /// <summary>
        /// Downloads the version asynchronously.
        /// </summary>
        /// <returns></returns>
        Task DownloadAsync();
    }
}
