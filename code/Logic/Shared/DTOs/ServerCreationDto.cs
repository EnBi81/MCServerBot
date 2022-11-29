using SharedPublic.DTOs;
using SharedPublic.Model;
using System.ComponentModel;
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
        [MinLength(IMinecraftServer.NAME_MIN_LENGTH)]
        [MaxLength(IMinecraftServer.NAME_MAX_LENGTH)]
        [DefaultValue("My new server")]
        public string? NewName { get; set; }

        /// <summary>
        /// The name of the server version to use.
        /// </summary>
        /// <example>1.19.2</example>
        [DefaultValue("1.19.2")]
        public string? Version { get; set; }

        /// <summary>
        /// Properties to apply when creating the server.
        /// </summary>
        public MinecraftServerCreationPropertiesDto? Properties { get; set; }
    }
}
