using Loggers;
using Shared.Exceptions;
using Shared.Model;
using System.Collections;

namespace Application.Minecraft.Versions
{
    /// <summary>
    /// Minecraft Version manager.
    /// </summary>
    internal partial class MinecraftVersionCollection : IMinecraftVersionCollection
    {
        /*
         * Use cases: 
         *   1. Get all versions
         *   2. Get MinecraftVersion instance by version (this["1.18.1"])
         *   3. (Re)load versions from a file. Example: 
         *      {
         *        "versions": [
         *          {
         *            "name": "The Wild Update",
         *            "version": "1.19.2",
         *            "fullRelease": "August 5, 2022",
         *            "downloadLink": "https://piston-data.mojang.com/v1/objects/f69c284232d7c7580bd89a5a4931c3581eae1378/server.jar"
         *          },
         *          {
         *            "name": "The Wild Update",
         *            "version": "1.19.1",
         *            "fullRelease": "July 27, 2022",
         *            "downloadLink": "https://piston-data.mojang.com/v1/objects/f69c284232d7c7580bd89a5a4931c3581eae1378/server.jar"
         *          },
         *        ]
         *      }
         *   4. Check if a version is downloaded
         *   5. Download version
         *   6. Get absolute path of a version jar file if downloaded
         */

        private readonly List<MinecraftVersion> _versions = new ();
        private readonly MinecraftLogger _logger;
        private readonly string _loggerSource = "version-manager";

        


        /// <summary>
        /// Initializes the version manager.
        /// </summary>
        /// <param name="versionsDir"></param>
        /// <param name="logger"></param>
        public MinecraftVersionCollection(string versionsDir, MinecraftLogger logger)
        {
            _versionsDir = versionsDir;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await LoadVersions();
            _logger.Log(_loggerSource, "Version manager initialized. Versions loaded: " + _versions.Count);

            if (_versions.Count == 0)
            {
                throw new MCInternalException("Couldn't load any version.");
            }
        }

        public async Task LoadAsync()
        {
            await CheckVersionsPython();
            await LoadVersions();
        }


        /// <inheritdoc/>
        public IMinecraftVersion? this[string? version] => 
            _versions.FirstOrDefault(v => v.Version == version);

        /// <inheritdoc/>
        public IMinecraftVersion Latest => GetSortedDescendingVersion(_versions).First();


        /// <inheritdoc/>
        public List<IMinecraftVersion> GetAll() => new (_versions);


        /// <inheritdoc/>
        public async Task DownloadVersionAsync(string version) => 
            await DownloadVersionThreadSafe(version);

        /// <inheritdoc/>
        public bool IsDownloaded(string version) => 
            File.Exists(GetAbsolutePath(version));




        /// <inheritdoc/>
        public IEnumerator<IMinecraftVersion> GetEnumerator() => _versions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _versions.GetEnumerator();
    }
}
