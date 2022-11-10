using Newtonsoft.Json;
using Shared.Exceptions;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Application.Minecraft.Versions
{
    internal partial class MinecraftVersionCollection
    {
        private readonly Dictionary<string, MinecraftVersion> _downloadingVersions = new();
        private readonly string _versionsDir;
        private readonly int _maxDownloadSimultaneously = 5;
        private const string _versionJsonFileName = "mc_version_list.json";

        private static List<T> GetSortedDescendingVersion<T>(List<T> list) where T : IMinecraftVersion
        {
            list.Sort((a, b) => -Version.Parse(a.Version).CompareTo(Version.Parse(b.Version)));
            return list;
        }
        
        /// <summary>
        /// Downloads the version manifest from Mojang and to the local file system.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        /// <exception cref="MCExternalException"></exception>
        private async Task DownloadVersionThreadSafe(string version)
        {
            if (IsDownloaded(version))
                return;

            var imcVersion = this[version];
            if (imcVersion is not MinecraftVersion mcVersion)
                throw new MCExternalException($"Cannot download version {version} for reason: {version} is not a registered version");
            
            AddVersionToDownloadingSync(mcVersion);
            DateTime start = DateTime.Now;
            _logger.Log(_loggerSource, $"Downloading version {version}...");


            using HttpClient client = new HttpClient();
            string filename = GetAbsolutePath(version);

            using var webStream = await client.GetStreamAsync(mcVersion.DownloadUrl);
            using var fileStream = File.Create(filename);
            await webStream.CopyToAsync(fileStream);

            _logger.Log(_loggerSource, $"Downloaded version {version}. Time taken: {DateTime.Now - start}");
            RemoveVersionFromDownloadingSync(mcVersion);
        }

        /// <summary>
        /// Adds a version to the downloading list. This method is thread safe.
        /// </summary>
        /// <param name="version"></param>
        /// <exception cref="MCExternalException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AddVersionToDownloadingSync(MinecraftVersion version)
        {
            if (_downloadingVersions.ContainsKey(version.Version))
            {
                if(_downloadingVersions.Count >= _maxDownloadSimultaneously)
                    throw new MCExternalException($"Cannot download more than {_maxDownloadSimultaneously} versions at once");
                _downloadingVersions.Add(version.Version, version);
            }
            else
                throw new MCExternalException($"Cannot add version {version.Version} to downloading versions because it is already downloading");
        }

        /// <summary>
        /// Removes the version from downloading versions. This method is thread safe.
        /// </summary>
        /// <param name="version"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void RemoveVersionFromDownloadingSync(MinecraftVersion version)
        {
            _downloadingVersions.Remove(version.Version);
        }

        /// <summary>
        /// Downloads the version informations from the web
        /// </summary>
        /// <returns></returns>
        private async Task DownloadVersionsFromNet()
        {
            var p = Process.Start(new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = "cmd.exe",
                WorkingDirectory = _versionsDir,
                arg
            });

            
        }


        /// <summary>
        /// Loads the versions from the versions.json file.
        /// </summary>
        /// <exception cref="MCInternalException"></exception>
        private async Task LoadVersions()
        {
            if (_versions.Any())
                return;

            string versionsFile = GetVersionFilePath();

            var text = await File.ReadAllTextAsync(versionsFile);
            var versions = JsonConvert.DeserializeObject<VersionJson>(text);

            if (versions is null)
                throw new MCInternalException("Couldn't find " + versionsFile);
            
            if (versions.Versions is null)
                throw new MCInternalException("Couldn't find versions in " + versionsFile);


            var filteredVersions = versions.Versions
                .Where(v => v.Name is not null)
                .Where(v => v.Version is not null && Regex.IsMatch(v.Version, @"^\d+(\.\d+)*$"))
                .Where(v => DateTime.TryParse(v.FullRelease, out _))
                .Where(v => Uri.IsWellFormedUriString(v.DownloadUrl, UriKind.Absolute));

            var incorrectVersions = versions.Versions.Except(filteredVersions);
            foreach (var incorrectVersion in incorrectVersions)
            {
                _logger.Error(_loggerSource, new MCInternalException("Incorrect version: " + incorrectVersion.ToString()));
            }


            var convertedVersions = versions.Versions.Select(mvJson => new MinecraftVersion(this) 
            {
                Name = mvJson.Name!,
                Version = mvJson.Version!,
                FullRelease = mvJson.FullRelease!,
                DownloadUrl = mvJson.DownloadUrl ?? ""
            });
        
            
            _versions.AddRange(convertedVersions);
        }

        private string GetVersionFilePath()
        {
            return Path.Combine(_versionsDir, _versionJsonFileName);
        }

        /// <summary>
        /// Gets the absolute path of a version (even if it doesn't exist).
        /// </summary>
        /// <param name="version">version (i.e. 1.19.1)</param>
        /// <returns></returns>
        internal string GetAbsolutePath(string version) => Path.GetFullPath(Path.Combine(_versionsDir, GetJarFileName(version)));

        /// <summary>
        /// Gets the jar file name of a version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static string GetJarFileName(string version) => $"server-{version}.jar";


        /// <summary>
        /// Used for deserializing the versions.json file.
        /// </summary>
        private class VersionJson
        {
            /// <summary>
            /// The versions.
            /// </summary>
            public List<MinecraftVersionJson>? Versions { get; set; }
        }

        /// <summary>
        /// Used for deserializing the versions.json file.
        /// </summary>
        private class MinecraftVersionJson
        {
            /// <summary>
            /// Name of the minecraft version.
            /// </summary>
            public string? Name { get; set; }
            /// <summary>
            /// Version of the minecraft server.
            /// </summary>
            public string? Version { get; set; }
            /// <summary>
            /// Full release.
            /// </summary>
            public string? FullRelease { get; set; }
            /// <summary>
            /// Download url.
            /// </summary>
            public string? DownloadUrl { get; set; }


            public override string ToString() => JsonConvert.SerializeObject(this);
        }
    }
}
