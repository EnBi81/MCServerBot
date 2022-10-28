using Shared.DTOs.Enums;

namespace Shared.DTOs
{
    public class UserEventData
    {
        public ulong Id { get; init; }
        public string Username { get; init; }
        public Platform Platform { get; init; }
    }
}
