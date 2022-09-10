using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MCWebServer.Discord.Commands;
using MCWebServer.Discord.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using CommandAttribute = MCWebServer.Discord.Commands.CommandAttribute;
using MCWebServer.PermissionControll;

namespace MCWebServer.Discord
{
    internal class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmdService;
        private IServiceProvider _services;
        private ulong _botOwnerId;

        public IReadOnlyDictionary<string, Func<SocketSlashCommand, Task>> Commands { get; } =
            new Dictionary<string, Func<SocketSlashCommand,Task>>()
            {
                ["start-server"] = MinecraftServerCommands.StartServer,
                ["stop-server"] = MinecraftServerCommands.ShutDownServer,
                ["grant-perm"] = PermissionCommands.GrantPermission,
                ["revoke-perm"] = PermissionCommands.RevokePermission,
                ["get-web-url"] = PermissionCommands.GetWebLoginPage,
                ["ping"] = ToolCommands.Ping,
            };


        public CommandHandler(DiscordSocketClient client, CommandService cmdService, IServiceProvider services)
        {
            _client = client;
            _cmdService = cmdService;
            _services = services;
        } 


        internal async Task InitializeAsync()
        {
            await _client.SetStatusAsync(UserStatus.Online);

            Config.Config.Instance.DiscordBotOwner = (await _client.GetApplicationInfoAsync()).Owner.Id;
            _botOwnerId = Config.Config.Instance.DiscordBotOwner;

            _client.Ready += () => SetUpSlashCommands();
            _cmdService.Log += async msg => await LogService.Log.LogAsync(msg);
            _client.SlashCommandExecuted += SlashCommandExecuted;
            _client.SelectMenuExecuted += MenuHandlers.HandleMenu;
        }

        

        public async Task RemoveAllCommands()
        {
            //delete all previously registered commands

            var commands = await _client.GetGlobalApplicationCommandsAsync();
            foreach (var command in commands)
                await command.DeleteAsync();
        }

        public async Task SetUpSlashCommands()
        {

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

                if (options.Count > 0)
                    command.Options = options;


                // register command
                await _client.CreateGlobalApplicationCommandAsync(command.Build());
            }
        }
        

        private async Task SlashCommandExecuted(SocketSlashCommand arg)
        {
            if (arg.User.IsBot)
                return;

            var userId = arg.User.Id;

            // check if the user has permission
            if (userId != _botOwnerId && !BotPermission.HasPermission(userId))
            { 
                await arg.RespondAsync("You don't have permission to use the commands. Please contact someone who has!", ephemeral: true);
                return;
            }


            // execute command
            var function = Commands[arg.Data.Name];
            await function(arg);
        }
    }
}
