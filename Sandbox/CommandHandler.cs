using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Sandbox.Commands;
using CommandAttribute = Sandbox.Commands.CommandAttribute;

namespace Sandbox
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;

        public IReadOnlyDictionary<string, Func<SocketSlashCommand, Task>> Commands { get; } =
            new Dictionary<string, Func<SocketSlashCommand,Task>>()
        {
            ["test"] = TestCommands.TestCommand
        };


        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;
        } 


        internal async Task InitializeAsync()
        {
            await _client.SetStatusAsync(UserStatus.Online);


            _client.Ready += () => SetUpSlashCommands();
            _client.SlashCommandExecuted += SlashCommandExecuted;
        }

        private async Task SetUpSlashCommands()
        {
            // delete all previously registered commands

            //var commands = await _client.GetGlobalApplicationCommandsAsync();
            //foreach (var command in commands)
            //    await command.DeleteAsync();



            foreach (var (name, commandFunction) in Commands)
            {
                // get description attribute
                CommandAttribute? attribute = commandFunction.Method.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute
                    ?? throw new NotImplementedException(name + " command does not have Command attribute");

                // get options
                var optionAttributes = commandFunction.Method.GetCustomAttributes(typeof(CommandOptionAttribute));
                List<SlashCommandOptionBuilder> options = new ();
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

                Console.WriteLine("command: " + command);

                if (options.Count > 0)
                    command.Options = options;
                Console.WriteLine("command: " + command);

                // register command
                var build = command.Build();
                await _client.CreateGlobalApplicationCommandAsync(build);
            }
        }

        private async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            if (arg.User.IsBot)
                return;

            var userId = arg.User.Id;


            // execute command
            var function = Commands[arg.Data.Name];
            await function(arg);
        }
    }
}
