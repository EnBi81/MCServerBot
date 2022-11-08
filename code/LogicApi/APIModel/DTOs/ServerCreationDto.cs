using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class ServerCreationDto
    {
        [Required]
        [MinLength(MinecraftServerDTO.NAME_MIN_LENGTH)]
        [MaxLength(MinecraftServerDTO.NAME_MAX_LENGTH)]
        public string? NewName { get; set; }
    }
}
