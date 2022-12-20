using SharedPublic.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SharedPublic.DTOs
{
    public class ModifyServerDto
    {
        /// <summary>
        /// The name of the server.
        /// </summary>
        /// <example>My Server</example>
        [MinLength(IMinecraftServer.NAME_MIN_LENGTH)]
        [MaxLength(IMinecraftServer.NAME_MAX_LENGTH)]
        [DefaultValue(null)]
        public string? NewName { get; set; }

        /// <summary>
        /// The name of the server version to use.
        /// </summary>
        /// <example>1.19.2</example>
        [DefaultValue(null)]
        public string? Version { get; set; }

        /// <summary>
        /// Properties to change
        /// </summary>
        public MinecraftServerPropertiesDto? Properties { get; set; }
        /// <summary>
        /// New icon of the server
        /// </summary>
        public string? Icon { get; set; }
    }
}
