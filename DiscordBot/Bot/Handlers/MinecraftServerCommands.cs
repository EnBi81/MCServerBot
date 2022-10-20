using Application.Minecraft;
using DataStorage.Interfaces;
using Discord.Interactions;
using DiscordBot.Bot.Handlers.Autocompletes;
using DiscordBot.Bot.Helpers;

namespace DiscordBot.Bot.Handlers
{
   
    public class MinecraftServerCommands : MCInteractionModuleBase
    {
        private IServerPark _serverPark;

        public MinecraftServerCommands(IServerPark serverPark, IDiscordEventRegister eventRegister) : base(eventRegister)
        {
            _serverPark = serverPark;
        }




        [SlashCommand("start-server", "Start the minecraft server")]
        public async Task StartServer([Summary("server-name", "Name of the server to start"), Autocomplete(typeof(ServerNameAutocomplete))] string serverName)
        {
            try
            {
                var user = await GetUser();
                await _serverPark.StartServer(serverName, user);

                await RespondAsync($"Starting **{serverName}**.");
            }
            catch (Exception e)
            {
                await RespondAsync($"Starting failed: **{e.Message}**.");
            }
        }

        

        [SlashCommand("stop-server", "Stop the minecraft server")]
        public async Task ShutDownServer()
        {
            try
            {
                var user = await GetUser();
                await _serverPark.StopActiveServer(user);

                await RespondAsync($"Shutting Down **{_serverPark.ActiveServer?.ServerName}**.");
            }
            catch (Exception ex)
            {
                await RespondAsync($"Shutting Down failed: **{ex.Message}**.", ephemeral: true);
            }
        }


        [SlashCommand("create-server", "Create a new server")]
        public async Task CreateServer([Summary("server-name", "Name of the new server")] string serverName)
        {
            try
            {
                var user = await GetUser();
                _serverPark.CreateServer(serverName, user);
                await RespondAsync($"Server **{serverName}** has been created");
            } catch (Exception e)
            {
                await RespondAsync($"Server **{serverName}** cannot be created: **{e.Message}**");
            }
        }

        [SlashCommand("rename-server", "Rename a server")]
        public async Task RenameServer([Summary("name", "Name of the server to rename"), Autocomplete(typeof(ServerNameAutocomplete))] string serverName,
                                        [Summary("new-name", "New name of the server")] string newName)
        {
            try
            {
                var user = await GetUser();
                await _serverPark.RenameServer(serverName, newName, user);

                await RespondAsync($"**{serverName}** has been renamed to **{newName}**");
            }
            catch (Exception e)
            {
                await RespondAsync($"**{serverName}** cannot be renamed: **{e.Message}**");
            }
        }
    }
}
