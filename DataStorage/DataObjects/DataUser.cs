using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataStorage.DataObjects
{
    public class DataUser
    {
        public static DataUser System { get; } = new ()
        {
            Id = 1,
            Username = "System",
            ProfilePicUrl = null,
            WebAccessToken = null
        };

        public ulong Id { get; internal init; }
        public string Username { get; internal init; } = null!;
        public string? ProfilePicUrl { get; internal init; }
        public string? WebAccessToken { get; internal init; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this,  new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
