using SharedPublic.DTOs;

namespace SharedPublic.Model
{
    public interface IMinecraftServerProperties
    {
        /// <summary>
        /// Dictionary of the visible properties of the minecraft server.
        /// </summary>
        public Dictionary<string, string> Properties { get; }

        /// <summary>
        /// Gets or sets the value of the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? this[string key] { get; }

        /// <summary>
        /// Gets the enumerator of the Properties property.
        /// </summary>
        /// <returns>the enumerator of the Properties property.</returns>
        public Dictionary<string, string>.Enumerator GetEnumerator();

        /// <summary>
        /// Update the minecraft server properties.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public Task UpdatePropertiesAsync(MinecraftServerPropertiesDto dto);
        /// <summary>
        /// Update the minecraft server properties.
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        public Task UpdatePropertiesAsync(Dictionary<string, string> props);
        /// <summary>
        /// Flushes all the property data and resets the state of the object.
        /// </summary>
        public void ClearProperties();
    }
}
