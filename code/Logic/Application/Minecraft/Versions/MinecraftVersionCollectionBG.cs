using Newtonsoft.Json;
using SharedPublic.Exceptions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Application.Minecraft.Versions
{
    // TODO: https://minecraft.fandom.com/wiki/Version_manifest.json
    internal partial class MinecraftVersionCollection
    {
        private const string _versionJsonFileName = "mc_version_list.json";
        private readonly Dictionary<string, Task> _downloadingVersions = new();
        private readonly string _versionsDir;
        private readonly int _maxDownloadSimultaneously = 5;
        private readonly string _resourceVersionFolder = 
            Path.Combine(Environment.GetEnvironmentVariable("RESOURCES_FOLDER") ?? throw new MCInternalException("RESOURCES_FOLDER not set!"), "python");

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

            while (true)
            {
                try
                {
                    // try to download the version
                    Task downloadTask = AddVersionToDownloadingSync(mcVersion, () => DownloadVersionFromNet(mcVersion));
                    await downloadTask;
                    RemoveVersionFromDownloadingSync(mcVersion);
                    break;
                }
                catch (MCExternalException e)
                {
                    // if there are too many downloadings, then get the first task and wait for it to finish
                    // so we can start to download our current version.

                    _logger.Log(_loggerSource, e.Message + " Waiting to finish downloading.");
                    await _downloadingVersions.Values.First();
                }
            }
        }

        private async Task DownloadVersionFromNet(MinecraftVersion mcVersion)
        {
            string version = mcVersion.Version;

            DateTime start = DateTime.Now;
            _logger.Log(_loggerSource, $"Downloading version {version}...");


            using HttpClient client = new HttpClient();
            string filename = GetAbsolutePath(version);
            string downloadFileName = filename + ".download";

            using var webStream = await client.GetStreamAsync(mcVersion.DownloadUrl);
            using var fileStream = File.Create(downloadFileName);
            await webStream.CopyToAsync(fileStream);
            fileStream.Close();

            File.Move(downloadFileName, filename);

            _logger.Log(_loggerSource, $"Downloaded version {version}. Time taken: {DateTime.Now - start}");
        }

        /// <summary>
        /// Adds a version to the downloading list. This method is thread safe.
        /// </summary>
        /// <param name="version"></param>
        /// <exception cref="MCExternalException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private Task AddVersionToDownloadingSync(MinecraftVersion version, Func<Task> downloadTask)
        {
            if (!_downloadingVersions.ContainsKey(version.Version))
            {
                if(_downloadingVersions.Count >= _maxDownloadSimultaneously)
                    throw new MCExternalException($"Cannot download more than {_maxDownloadSimultaneously} versions at once.");

                Task download = downloadTask();
                _downloadingVersions.Add(version.Version, download);
                return download;
            }
            else
                return _downloadingVersions[version.Version];
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
        private async Task CheckVersionsPython()
        {
            _logger.Log(_loggerSource, "Downloading version list from Mojang...");

            await ExecuteScript("dependency_downloader.py");
            await ExecuteScript("mc_version_list_downloader.py");
            

            async Task ExecuteScript(string filename)
            {
                var p = Process.Start(new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    FileName = "cmd.exe",
                    WorkingDirectory = _resourceVersionFolder,
                    RedirectStandardInput = true
                });

                if (p == null)
                    throw new MCInternalException("Failed to start python script: " + filename);

                await p.StandardInput.WriteLineAsync($"python {filename} & exit");
                // TODO: maybe kiirni a logot
                await p.WaitForExitAsync();
            }
        }


        /// <summary>
        /// Loads the versions from the versions.json file.
        /// </summary>
        /// <exception cref="MCInternalException"></exception>
        private async Task LoadVersions()
        {
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
                .Where(v => Uri.IsWellFormedUriString(v.DownloadLink, UriKind.Absolute))
                .Where(v => v.DownloadLink!.StartsWith("https"))
                // select only the versions which are not already loaded to the collection
                .Where(v => !_versions.Any(version => version.Version == v.Version))
                // be sure all the versions are distinct
                .DistinctBy(version => version.Version);
            


            var convertedVersions = filteredVersions.Select(mvJson => new MinecraftVersion(this) 
            {
                Name = mvJson.Name!,
                Version = mvJson.Version!,
                FullRelease = mvJson.FullRelease!,
                DownloadUrl = mvJson.DownloadLink!
            });
        
            
            _versions.AddRange(convertedVersions);
        }

        private string GetVersionFilePath()
        {
            return Path.Combine(_resourceVersionFolder, _versionJsonFileName);
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
            public string? DownloadLink { get; set; }


            public override string ToString() => JsonConvert.SerializeObject(this);
        }
    }
}
