using Newtonsoft.Json;
using Shared.Exceptions;
using System.Runtime.CompilerServices;

namespace Application.Minecraft.Versions
{
    internal partial class MinecraftVersionCollection
    {
        private readonly Dictionary<string, MinecraftVersion> _downloadingVersions = new();
        private readonly string _versionsDir;

        private async Task DownloadVersionThreadSafe(string version)
        {
            if (IsDownloaded(version))
                return;

            var imcVersion = this[version];
            if (imcVersion is not MinecraftVersion mcVersion)
                throw new MCExternalException($"Cannot download version {version} for reason: {version} is not a registered version");
            
            AddVersionToDownloading(mcVersion);
            

            using HttpClient client = new HttpClient();
            string filename = GetAbsolutePath(version);

            using var webStream = await client.GetStreamAsync(mcVersion.DownloadUrl);
            using var fileStream = File.Create(filename);
            await webStream.CopyToAsync(fileStream);

            RemoveVersionFromDownloading(mcVersion);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AddVersionToDownloading(MinecraftVersion version)
        {
            if(_downloadingVersions.ContainsKey(version.Version))
                _downloadingVersions.Add(version.Version, version);
            else
                throw new MCExternalException($"Cannot add version {version.Version} to downloading versions because it is already downloading");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveVersionFromDownloading(MinecraftVersion version)
        {
            _downloadingVersions.Remove(version.Version);
        }


        private void LoadVersions()
        {
            string versionsFile = Path.Combine(_versionsDir, "versions.json");

            var text = File.ReadAllText(versionsFile);
            var versions = JsonConvert.DeserializeObject<VersionJson>(text);

            if (versions is null)
                throw new MCInternalException("Couldn't find " + versionsFile);
            
            if (versions.Versions is null)
                throw new MCInternalException("Couldn't find versions in " + versionsFile);

            var convertedVersions = versions.Versions.Select(mvJson => new MinecraftVersion(this, GetAbsolutePath, IsDownloaded) 
            {
                Name = mvJson.Name ?? throw new MCInternalException("MinecraftVersion.Name is null"),
                Version = mvJson.Version ?? throw new MCInternalException("MinecraftVersion.Version is null"),
                FullRelease = mvJson.FullRelease ?? throw new MCInternalException("MinecraftVersion.FullRelease is null"),
                DownloadUrl = mvJson.DownloadUrl ?? throw new MCInternalException("MinecraftVersion.DownloadUrl is null")
            });
        
            
            _versions.AddRange(convertedVersions);
        }


        private string GetAbsolutePath(string version) => Path.Combine(_versionsDir, GetJarFileName(version));

        private static string GetJarFileName(string version) => $"server-{version}.jar";
        

        private class VersionJson
        {
            public List<MinecraftVersionJson>? Versions { get; set; }
        }

        private class MinecraftVersionJson
        {
            public string? Name { get; set; }
            public string? Version { get; set; }
            public string? FullRelease { get; set; }
            public string? DownloadUrl { get; set; }
        }
    }
}
