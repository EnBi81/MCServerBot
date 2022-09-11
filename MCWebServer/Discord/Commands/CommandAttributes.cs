using Discord;
using System;
using System.Collections.Generic;
using System.Security;

namespace MCWebServer.Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class CommandAttribute : Attribute
    {
        public string Description { get; }

        public CommandAttribute(string description)
        {
            Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class CommandOptionAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public bool IsRequired { get; }
        public ApplicationCommandOptionType Type { get; }


        public CommandOptionAttribute(string name, string desc, ApplicationCommandOptionType type, bool isRequired = true)
        {
            Name = name;
            Description = desc;
            Type = type;
            IsRequired = isRequired;
        }

        public static explicit operator SlashCommandOptionBuilder(CommandOptionAttribute attr)
        {
            SlashCommandOptionBuilder builder = new()
            {
                Name = attr.Name,
                Description = attr.Description,
                IsRequired = attr.IsRequired,
                Type = attr.Type
            };

            return builder;
        }
    }
}
