using Shared.Attributes;
using Shared.Exceptions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Shared.DTOs
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
        
        
        private void ValidateValue(PropertyInfo property, ref object value)
        {
            var propName = property.Name;
            var propType = property.PropertyType;

            // checks for bool
            if (propType == typeof(bool?))
            {
                if (value is not bool b)
                    throw new MCInternalException("Value must be a boolean for " + propName);

                value = b ? "true" : "false";
            }
            // checks for int
            else if (propType == typeof(int?))
            {
                if (value is not int number)
                    throw new MCInternalException("Value must be an integer for " + propName);

                var rangeAttr = property.GetCustomAttribute<RangeAttribute>();
                if (rangeAttr == null || rangeAttr.Minimum is not int || rangeAttr.Maximum is not int)
                    throw new MCInternalException("No RangeAttribute found on property " + propName);

                if (number < (int)rangeAttr.Minimum || number > (int)rangeAttr.Maximum)
                    throw new MCExternalException($"Unexpected value for {propName}: '{number}'. Value must be between " + rangeAttr.Minimum + " and " + rangeAttr.Maximum + ".");
            }
            // checks for string
            else if (propType == typeof(string))
            {
                if (value is not string text)
                    throw new MCInternalException("Value must be a string for " + property.Name);

                var maxLengthAttr = property.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttr != null && text.Length > maxLengthAttr.Length)
                    throw new MCExternalException($"Unexpected value for {propName}: '{text}'. Value must be at most {maxLengthAttr.Length} characters long.");

                var validValuesAttr = property.GetCustomAttribute<ValidValuesAttribute>();
                if (validValuesAttr != null && !validValuesAttr.ValidValues.Contains(text))
                    throw new MCExternalException($"Unexpected value for {propName}: '{text}'. Value must be one of the following: " + string.Join(", ", validValuesAttr.ValidValues));
            }
            // unknown type
            else
            {
                throw new MCInternalException($"Property type {property.PropertyType} is not supported for {property.Name}.");
            }
        }

        protected Dictionary<string, string> ValidateAndRetrieveData(bool setAllDefaultValues)
        {
            var properties = GetType().GetProperties();

            var values = new Dictionary<string, string>();

            // get all properties
            foreach (var prop in properties)
            {
                // get basic attributes
                var displayAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
                var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();

                if (displayAttr == null || displayAttr.DisplayName == null)
                    continue;

                string? key = displayAttr.DisplayName;
                object? value = prop.GetValue(this);

                if(setAllDefaultValues && defaultAttr != null)
                    value ??= defaultAttr.Value;

                if (value == null || key == null)
                    continue;

                ValidateValue(prop, ref value);

                values.Add(key, value.ToString()!);
            }


            return values;
        }


        public virtual Dictionary<string, string> ValidateAndRetrieveData() 
            => ValidateAndRetrieveData(false);


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
        [DefaultValue("normal")]
        [ValidValues("peaceful", "easy", "normal", "hard")]
        public string? Difficulty { get; set; }

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
        [DefaultValue("survival")]
        [ValidValues("survival", "creative", "adventure", "spectator")]
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
