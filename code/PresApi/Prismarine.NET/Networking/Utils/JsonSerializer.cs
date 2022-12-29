using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;
using System.Text.Json;

namespace Prismarine.NET.Networking.Utils
{
    /// <summary>
    /// A class that serializes and deserializes objects to and from JSON.
    /// </summary>
    internal class JsonSerializer
    {
        
        private readonly JsonSerializerSettings _settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
        };


        public JsonSerializer(IContainer container)
        {
            
        }

#pragma warning disable CA1822 // Mark members as static

        /// <summary>
        /// Serializes an object to JSON.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object? obj)
        {
            return JsonConvert.SerializeObject(obj);
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
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }

        public void Populate<T>(string? json, object obj)
        {
            if (json is null)
                throw new ArgumentNullException($"The {nameof(json)} string is null.");
            JsonConvert.PopulateObject(json, obj);
        }

#pragma warning restore CA1822
    }
}

