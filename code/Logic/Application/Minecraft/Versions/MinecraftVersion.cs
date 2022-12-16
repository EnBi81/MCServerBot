using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Versions;

/// <summary>
/// Internal class to store Minecraft version
/// </summary>
internal class MinecraftVersion : IMinecraftVersion
{
    /// <inheritdoc/>
    public string Name { get; set; } = null!;
    /// <inheritdoc/>
    public string Version { get; set; } = null!;
    /// <summary>
    /// Full release stored as a <see cref="string"/>
    /// </summary>
    public string FullRelease { get; set; } = null!;
    /// <inheritdoc/>
    public DateTime ReleaseDate => DateTime.Parse(FullRelease);
    /// <inheritdoc/>
    public string DownloadUrl { get; set; } = null!;
    /// <inheritdoc/>
    public bool IsDownloaded => AbsoluteJarPath is not null;
    /// <inheritdoc/>
    public string? AbsoluteJarPath => _versionCollection.IsDownloaded(Version) ? _versionCollection.GetAbsolutePath(Version) : null;

    

    private readonly MinecraftVersionCollection _versionCollection;
    
    internal MinecraftVersion(MinecraftVersionCollection coll)
    {
        _versionCollection = coll;
    }


    /// <inheritdoc/>
    public void Download() => DownloadAsync().GetAwaiter().GetResult();
    /// <inheritdoc/>
    public async Task DownloadAsync() => await _versionCollection.DownloadVersionAsync(Version);
}
