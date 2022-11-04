using Shared.DTOs.Enums;

namespace Shared.DTOs
{
    public struct UserEventData
    {
        private ulong? _id;
        public ulong Id { get => _id ?? DataUser.System.Id; init => _id = value; }

        private string _username;
        public string Username { get => _username ?? DataUser.System.Username; init => _username = value; }

        private Platform? _platform;
        public Platform Platform { get => _platform ?? Platform.SystemAdmin ; init => _platform = value; }
    }
}
