using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class UserDataDto
    {
        [Required]
        public ulong Id { get; set; }
        [Required]
        public string? DiscordName { get; set; }
        [Required]
        public string? ProfilePic { get; set; }
    }
}
