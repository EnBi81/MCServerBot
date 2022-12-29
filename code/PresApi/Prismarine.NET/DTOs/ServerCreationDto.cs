using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prismarine.NET.DTOs
{
    public class ServerCreationDto
    {
        /// <summary>
        /// New name of the server
        /// </summary>
        public required string NewName { get; set; }
        /// <summary>
        /// Version of the server
        /// </summary>
        public string? Version { get; set; }
        /// <summary>
        /// Icon of the server
        /// </summary>
        public string? ServerIcon { get; set; }
        /// <summary>
        /// Properties of the server
        /// </summary>
        public MinecraftServerCreationPropertiesDto? Properties { get; set; }
    }
}
