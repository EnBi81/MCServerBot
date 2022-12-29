using Newtonsoft.Json;
using System.Text.Json;

namespace Prismarine.NET.Networking.Utils
{
    /// <summary>
    /// A class that serializes and deserializes objects to and from JSON.
    /// </summary>
    internal class JsonSerializer
    {

        private static readonly JsonSerializerOptions _settings = new ()
        {
            AllowTrailingCommas = true, 
            PropertyNameCaseInsensitive = true,
        };

        

#pragma warning disable CA1822 // Mark members as static

        /// <summary>
        /// Serializes an object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object? obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// Deserializes an object from JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T? Deserialize<T>(string? json)
        {
            if (json is null)
                throw new ArgumentNullException($"The {nameof(json)} string is null.");
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, _settings);
        }

#pragma warning restore CA1822
    }
}
