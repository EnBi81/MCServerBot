namespace Shared.Model
{
    public interface IMinecraftServerProperties
    {
        /// <summary>
        /// Dictionary of each of the property key and value.
        /// </summary>
        public Dictionary<string, string> Properties { get; }

        /// <summary>
        /// Gets or sets the value of the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? this[string key] { get; set; }

        /// <summary>
        /// Gets the enumerator of the Properties property.
        /// </summary>
        /// <returns>the enumerator of the Properties property.</returns>
        public Dictionary<string, string>.Enumerator GetEnumerator();
    }
}
