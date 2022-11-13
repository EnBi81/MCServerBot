using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace APIModel.DTOs
{
    public class MinecraftServerPropertiesDto
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

        /// <summary>
        /// <para>Allows users to use flight on the server while in Survival mode, if they have a mod that provides flight installed.</para>
        /// <para>With allow-flight enabled, griefers may become more common, because it makes their work easier.In Creative mode, this has no effect.</para>
        /// <para><b>false</b> - Flight is not allowed(players in air for at least 5 seconds get kicked).</para>
        /// <para><b>true</b> - Flight is allowed, and used if the player has a fly mod installed.</para>
        /// </summary>
        /// <example>true</example>
        [DisplayName("allow-flight")]
        [DefaultValue(true)]
        public bool? AllowFlight { get; set; }

        /// <summary>
        /// Defines the difficulty (such as damage dealt by mobs and the way hunger and poison affects players) of the server.
        /// <para>If a legacy difficulty number is specified, it is silently converted to a difficulty name.</para>
        /// <list type="bullet">
        /// peaceful(0)
        /// </list>
        /// <list type="bullet">
        /// easy(1)
        /// </list>
        /// <list type="bullet">
        /// normal(2)
        /// </list>
        /// <list type="bullet">
        /// hard(3)
        /// </list>
        /// </summary>
        /// <example>2</example>
        [DisplayName("difficulty")]
        [Range(0, 3)]
        [DefaultValue(2)] // normal
        public int? Difficulty { get; set; }

        /// <summary>
        /// Enables command blocks
        /// </summary>
        /// <example>true</example>
        [DisplayName("enable-command-block")]
        [DefaultValue(true)]
        public bool? EnableCommandBlock { get; set; }


        /// <summary>
        /// If set to true, players without a Mojang-signed public key will not be able to connect to the server.
        /// </summary>
        /// <example>false</example>
        [DisplayName("enforce-secure-profile")]
        [DefaultValue(false)]
        public bool? EnforceSecureProfile { get; set; }

        /// <summary>
        /// Defines the mode of gameplay.
        /// <para>If a legacy gamemode number is specified, it is silently converted to a gamemode name.</para>
        /// <list type="bullet">
        /// survival (0)
        /// </list>
        /// <list type="bullet">
        /// creative (1)
        /// </list>
        /// <list type="bullet">
        /// adventure (2)
        /// </list>
        /// <list type="bullet">
        /// spectator (3)
        /// </list>
        /// </summary>
        /// <example>0</example>
        [DisplayName("gamemode")]
        [Range(0, 3)]
        [DefaultValue(0)] // survival
        public string? Gamemode { get; set; }

        /// <summary>
        /// If set to true, server difficulty is ignored and set to hard and players are set to spectator mode if they die.
        /// </summary>
        /// <example>false</example>
        [DisplayName("hardcore")]
        [DefaultValue(false)]
        public bool? Hardcore { get; set; }

        /// <summary>
        /// This sets the maximum possible size in blocks, expressed as a radius, that the world border can obtain. Setting the world border bigger causes the commands to complete successfully but the actual border does not move past this block limit. Setting the max-world-size higher than the default doesn't appear to do anything.
        /// </summary>
        /// <example>29999984</example>
        [DisplayName("max-world-size")]
        [Range(1, 29999984)]
        [DefaultValue(29999984)]
        public int? MaxWorldSize { get; set; }

        /// <summary>
        /// This is the message that is displayed in the server list of the client, below the name.
        /// </summary>
        /// <example>A Minecraft Server</example>
        [DisplayName("motd")]
        [MaxLength(59)]
        [DefaultValue("A Minecraft Server")]
        public string? Motd { get; set; }

        /// <summary>
        /// Enable PvP on the server. Players shooting themselves with arrows receive damage only
        /// </summary>
        /// <example>true</example>
        [DisplayName("pvp")]
        [DefaultValue(true)]
        public bool? Pvp { get; set; }

        /// <summary>
        /// Sets the maximum distance from players that living entities may be located in order to be updated by the server, measured in chunks in each direction of the player (radius, not diameter). If entities are outside of this radius, then they will not be ticked by the server nor will they be visible to players.
        /// </summary>
        /// <example>10</example>
        [DisplayName("simulation-distance")]
        [Range(3, 32)]
        [DefaultValue(10)]
        public int? SimulationDistance { get; set; }

        /// <summary>
        /// Determines if monsters can spawn.
        /// </summary>
        /// <example>true</example>
        [DisplayName("spawn-monsters")]
        [DefaultValue(true)]
        public bool? SpawnMonsters { get; set; }

        /// <summary>
        /// Determines the side length of the square spawn protection area as 2x+1. Setting this to 0 disables the spawn protection. A value of 1 protects a 3×3 square centered on the spawn point. 2 protects 5×5, 3 protects 7×7, etc. This option is not generated on the first server start and appears when the first player joins. If there are no ops set on the server, the spawn protection is disabled automatically as well.
        /// </summary>
        /// <example>0</example>
        [DisplayName("spawn-protection")]
        [Range(0, 30000)]
        [DefaultValue(0)]
        public int? SpawnProtection { get; set; }

        /// <summary>
        /// Sets the amount of world data the server sends the client, measured in chunks in each direction of the player (radius, not diameter). It determines the server-side viewing distance.
        /// 10 is the default/recommended.If the player has major lag, this value is recommended to be reduced.
        /// </summary>
        /// <example>10</example>
        [DisplayName("view-distance")]
        [Range(3, 32)]
        [DefaultValue(10)]
        public int? ViewDistance { get; set; }

        /// <summary>
        /// Enables a whitelist on the server.
        /// With a whitelist enabled, users not on the whitelist cannot connect.Intended for private servers, such as those for real-life friends or strangers carefully selected via an application process, for example.
        /// </summary>
        /// <example>false</example>
        [DisplayName("white-list")]
        [DefaultValue(false)]
        public bool? WhiteList { get; set; }
        
    }
}
