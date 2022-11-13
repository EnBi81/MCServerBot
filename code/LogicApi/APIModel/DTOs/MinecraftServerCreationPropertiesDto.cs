using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace APIModel.DTOs
{
    public class MinecraftServerCreationPropertiesDto : MinecraftServerPropertiesDto
    {
        /// <summary>
        /// Sets a world seed for the player's world, as in Singleplayer. The world generates with a random seed if left blank.
        /// </summary>
        /// <example>5402425475af6a4fr5er4asg</example>
        [DisplayName("level-seed")]
        [MaxLength(100)]
        public string? LevelSeed { get; set; }

        /// <summary>
        /// Determines the world preset that is generated.
        /// <list type="bullet">
        /// <item>normal</item>
        /// <item>flat</item>
        /// <item>large_biomes</item>
        /// <item>amplified</item>
        /// <item>single_biome_surface</item>
        /// </list>
        /// </summary>
        /// <example>normal</example>
        [DisplayName("level-type")]
        [DefaultValue("minecraft:default")]
        public string? LevelType { get; set; }
    }
}
