using Shared.DTOs.Enums;

namespace SharedAuth.DTOs
{
    public class LoginDto
    {
        public string? Token { get; set; }
        public string? Platform { get; set; }
    }
}
