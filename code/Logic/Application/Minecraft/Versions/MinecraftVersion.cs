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

        public bool IsDownloaded => _isDownloaded(Version);
        public string? AbsoluteJarPath => _getAbsolutePath(Version);


        private readonly MinecraftVersionCollection _versionCollection;
        private readonly Func<string, string?> _getAbsolutePath;
        private readonly Func<string, bool> _isDownloaded;

        internal MinecraftVersion(MinecraftVersionCollection coll, Func<string, string?> getAbsolutePath, Func<string, bool> isDownloaded)
        {
            _versionCollection = coll;
            _getAbsolutePath = getAbsolutePath;
            _isDownloaded = isDownloaded;
        }


        public void Download() => _versionCollection.DownloadVersionAsync(Version).GetAwaiter().GetResult();
        public async Task DownloadAsync() => await _versionCollection.DownloadVersionAsync(Version);
    }
}
