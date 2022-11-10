using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class ModifyServerDto
    {
        [MinLength(MinecraftServerDTO.NAME_MIN_LENGTH)]
        [MaxLength(MinecraftServerDTO.NAME_MAX_LENGTH)]
        public string? NewName { get; set; }

        public string? Version { get; set; }
    }
}
