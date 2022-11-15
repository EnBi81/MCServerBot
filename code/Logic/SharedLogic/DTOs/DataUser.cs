using SharedPublic.DTOs;
using System.Text.Json;

namespace SharedLogic.DTOs
{
    public class DataUser
    {
        public static DataUser System { get; } = new ()
        {
            Id = 1,
            Username = "System",
            ProfilePicUrl = null,
            UserType = DataUserType.System
        };

        public ulong Id { get; init; }
        public string Username { get; init; } = null!;
        public string? ProfilePicUrl { get; init; }
        public DataUserType UserType { get; init; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this,  new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
