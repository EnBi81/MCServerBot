using Newtonsoft.Json;
using Shared.Exceptions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
            
            AddVersionToDownloadingSync(mcVersion);
            

            using HttpClient client = new HttpClient();
            string filename = GetAbsolutePath(version);

            using var webStream = await client.GetStreamAsync(mcVersion.DownloadUrl);
            using var fileStream = File.Create(filename);
            await webStream.CopyToAsync(fileStream);

            RemoveVersionFromDownloadingSync(mcVersion);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AddVersionToDownloadingSync(MinecraftVersion version)
        {
            if(_downloadingVersions.ContainsKey(version.Version))
                _downloadingVersions.Add(version.Version, version);
            else
                throw new MCExternalException($"Cannot add version {version.Version} to downloading versions because it is already downloading");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveVersionFromDownloadingSync(MinecraftVersion version)
        {
            _downloadingVersions.Remove(version.Version);
        }


        private void LoadVersions()
        {
            if (_versions.Any())
                return;

            string versionsFile = Path.Combine(_versionsDir, "versions.json");

            var text = File.ReadAllText(versionsFile);
            var versions = JsonConvert.DeserializeObject<VersionJson>(text);

            if (versions is null)
                throw new MCInternalException("Couldn't find " + versionsFile);
            
            if (versions.Versions is null)
                throw new MCInternalException("Couldn't find versions in " + versionsFile);


            var filteredVersions = versions.Versions
                .Where(v => v.Name is not null)
                .Where(v => v.Version is not null && Regex.IsMatch(v.Version, @"^\d+(\.\d+)*$"))
                .Where(v => v.FullRelease is not null && DateTime.TryParse(v.FullRelease, out _));
            //.Where(v => v.DownloadUrl is not null && Uri.IsWellFormedUriString(v.DownloadUrl, UriKind.Absolute));

            var incorrectVersions = versions.Versions.Except(filteredVersions);
            foreach (var incorrectVersion in incorrectVersions)
            {
                _logger.Error("version-manager", new MCInternalException("Incorrect version: " + incorrectVersion.ToString()));
            }


            var convertedVersions = versions.Versions.Select(mvJson => new MinecraftVersion(this, GetAbsolutePath, IsDownloaded) 
            {
                Name = mvJson.Name!,
                Version = mvJson.Version!,
                FullRelease = mvJson.FullRelease!,
                DownloadUrl = mvJson.DownloadUrl ?? ""
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


            public override string ToString() => JsonConvert.SerializeObject(this);
        }
    }
}
