using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class ServerCreationDto
    {
        /// <summary>
        /// The name of the server.
        /// </summary>
        /// <example>My Server</example>
        [Required]
        [MinLength(MinecraftServerDTO.NAME_MIN_LENGTH)]
        [MaxLength(MinecraftServerDTO.NAME_MAX_LENGTH)]
        public string? NewName { get; set; }

        /// <summary>
        /// The name of the server version to use.
        /// </summary>
        /// <example>1.19.2</example>
        public string? Version { get; set; }
    }
}
