using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using MCWebServer.Discord.Handlers;
using System.Security;

namespace MCWebServer.Discord.Commands
{
    public class CommandSetup
    {
        public static async Task RemoveCommands(DiscordSocketClient client, bool removeAll)
        {
            //delete all previously registered commands

            var commands = await client.GetGlobalApplicationCommandsAsync();

            foreach (var command in commands)
            {
                if (!removeAll)
                {
                    if (CommandHandlers.Commands.ContainsKey(command.Name))
                        continue;
                }

                await command.DeleteAsync();
            }
        }

        public static async Task SetUpSlashCommands(DiscordSocketClient client, IReadOnlyDictionary<string, Func<SocketSlashCommand, Task>> commands, bool allCommands = false)
        {
            // get all registered commands
            IReadOnlyCollection<SocketApplicationCommand> registeredCommands = allCommands ? await client.GetGlobalApplicationCommandsAsync() : null;

            foreach (var (name, commandFunction) in commands)
            {
                //skip if command is already registered
                if (!allCommands)
                {
                    if (registeredCommands != null && registeredCommands.Any(x => x.Name == name))
                        continue;
                }

                // get description attribute
                CommandAttribute? attribute = commandFunction.Method.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute
                    ?? throw new NotImplementedException(name + " command does not have Command attribute");

                // get options
                var optionAttributes = commandFunction.Method.GetCustomAttributes(typeof(CommandOptionAttribute));
                List<SlashCommandOptionBuilder> options = new();
                foreach (CommandOptionAttribute option in optionAttributes.Cast<CommandOptionAttribute>())
                {
                    options.Add((SlashCommandOptionBuilder)option);
                }


                // create command
                var command = new SlashCommandBuilder()
                {
                    Name = name,
                    Description = attribute.Description
                };
                
                if (options.Count > 0)
                    command.AddOptions(options.ToArray());


                // register command
                var builtCommand = command.Build();
                await client.CreateGlobalApplicationCommandAsync(builtCommand);
            }
        }
    }
}
