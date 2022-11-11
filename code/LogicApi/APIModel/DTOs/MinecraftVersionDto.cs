using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIModel.DTOs
{
    public class MinecraftVersionDto
    {
        /// <summary>
        /// Name of the version
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Version (etc. 1.19.2)
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Release date
        /// </summary>
        public DateTime ReleaseDate { get; set; }
        /// <summary>
        /// Download url for the server.jar
        /// </summary>
        public string DownloadUrl { get; set; }
        /// <summary>
        /// Gets if the version is downloaded locally
        /// </summary>
        public bool IsDownloaded { get; set; }
    }
}
