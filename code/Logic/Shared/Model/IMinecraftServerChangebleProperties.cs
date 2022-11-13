using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shared.Model
{
    public interface IMinecraftServerChangebleProperties
    {
        // https://minecraft.fandom.com/wiki/Server.properties

        /*
             allow-flight: default true
             difficulty: default normal, peaceful(0)-easy-normal-hard(3)
             enable-command-block: default true
             enforce-secure-profile: default false
             gamemode: at creation, default survival, survival(0)-creative-adventure-spectator
             hardcore: default false
             level-seed: at creation, lehet null, max 100 karakter
             level-type: at creation, default minecraft\:normal, normal-flat-large_biomes-amplified-single_biome_surface
             max-world-size: default 29999984, 1-29999984
             motd: default "A Minecraft Server", max length: 59 chars
             pvp: default true
             simulation-distance: default 10, 3-32
             spawn-monsters: default true
             spawn-protection: default 0, 0-30000
             view-distance: default 10, 3-32
             white-list: default false
         */

        [DefaultValue(true)]
        public bool? AllowFlight { get; set; }

        [Range(0, 3)]
        [DefaultValue(2)] // normal
        public int? Difficulty { get; set; }

        [DefaultValue(true)]
        public bool? EnableCommandBlock { get; set; }

        [DefaultValue(true)]
        public bool? EnforceSecureProfile { get; set; }

        [Range(0, 3)]
        [DefaultValue(0)] // survival
        public string? Gamemode { get; set; }

        [DefaultValue(false)]
        public bool? Hardcore { get; set; }

        [MaxLength(100)]
        public string? LevelSeed { get; set; }

        [DefaultValue("minecraft:default")]
        public string? LevelType { get; set; }

        [Range(1, 29999984)]
        [DefaultValue(29999984)]
        public int? MaxWorldSize { get; set; }

        [MaxLength(59)]
        [DefaultValue("A Minecraft Server")]
        public string? Motd { get; set; }

        [DefaultValue(true)]
        public bool? Pvp { get; set; }

        [Range(3, 32)]
        [DefaultValue(10)]
        public int? SimulationDistance { get; set; }

        [DefaultValue(true)]
        public bool? SpawnMonsters { get; set; }

        [Range(0, 30000)]
        [DefaultValue(0)]
        public int? SpawnProtection { get; set; }

        [Range(3, 32)]
        [DefaultValue(10)]
        public int? ViewDistance { get; set; }

        [DefaultValue(false)]
        public bool? WhiteList { get; set; }
        
    }
}
