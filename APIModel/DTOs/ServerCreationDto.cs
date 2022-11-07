using Shared.Model;
using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class ServerCreationDto
    {
        [Required]
        [MinLength(IMinecraftServer.NAME_MIN_LENGTH)]
        [MaxLength(IMinecraftServer.NAME_MAX_LENGTH)]
        public string? NewName { get; set; }
    }
}
