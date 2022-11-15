using Application.Minecraft.Versions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Model
{
    public interface IMinecraftVersionCollection : IEnumerable<IMinecraftVersion>
    {
        /// <summary>
        /// Gets the version instance by version.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public IMinecraftVersion? this[string? version] { get; }

        /// <summary>
        /// Gets the latest minecraft version.
        /// </summary>
        public IMinecraftVersion Latest { get; }

        /// <summary>
        /// Initializes the collection. Throws exception if couldn't load any version.
        /// </summary>
        /// <returns></returns>
        public Task InitializeAsync();

        /// <summary>
        /// Loads new versions from the web.
        /// </summary>
        /// <returns></returns>
        public Task LoadAsync();

        /// <summary>
        /// Gets all the versions.
        /// </summary>
        /// <returns></returns>
        public List<IMinecraftVersion> GetAll();


        /// <summary>
        /// Downloads a version asynchronously.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public Task DownloadVersionAsync(string version);

        /// <summary>
        /// Checks if a version is downloaded locally.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsDownloaded(string version);
    }
}
