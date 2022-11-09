using System.Collections;

namespace Application.Minecraft.Versions
{
    internal partial class MinecraftVersionCollection : IEnumerable<IMinecraftVersion>
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
        

        public MinecraftVersionCollection(string versionsDir)
        {
            _versionsDir = versionsDir;
            LoadVersions();
        }

        


        
        public IMinecraftVersion? this[string version] => 
            _versions.FirstOrDefault(v => v.Version == version);
        
        public List<IMinecraftVersion> GetAll() => new (_versions);


        public async Task DownloadVersionAsync(string version) => 
            await DownloadVersionThreadSafe(version);

        public bool IsDownloaded(string version) => 
            File.Exists(GetAbsolutePath(version));
        
        



        public IEnumerator<IMinecraftVersion> GetEnumerator() => _versions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _versions.GetEnumerator();
    }
}
