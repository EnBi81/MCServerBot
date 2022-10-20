using Application.Minecraft;
using DataStorage.Interfaces;
using Discord.Interactions;
using DiscordBot.Bot.Handlers.Autocompletes;

namespace DiscordBot.Bot.Handlers
{
   
    public class MinecraftServerCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private IServerPark _serverPark;
        private IDiscordEventRegister _eventRegister;

        public MinecraftServerCommands(IServerPark serverPark, IDiscordEventRegister eventRegister)
        {
            _serverPark = serverPark;
            _eventRegister = eventRegister;
        }




        [SlashCommand("start-server", "Start the minecraft server")]
        public async Task StartServer([Summary("server-name", "Name of the server to start"), Autocomplete(typeof(ServerNameAutocomplete))] string serverName)
        {
            try
            {
                ulong serverId = _serverPark.StartServer(serverName, Context.User.Username);
                _eventRegister.StartServer(Context.User.Id, serverId);

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
                ulong serverId = _serverPark.StopActiveServer(Context.User.Username);
                _eventRegister.StopServer(Context.User.Id, serverId);

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
                var mcServer = _serverPark.CreateServer(serverName);
                _eventRegister.CreateServer(Context.User.Id, mcServer.ServerName, mcServer.StorageBytes);

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
                ulong serverId = _serverPark.RenameServer(serverName, newName);
                _eventRegister.RenameServer(Context.User.Id, serverId, newName);

                await RespondAsync($"**{serverName}** has been renamed to **{newName}**");
            }
            catch (Exception e)
            {
                await RespondAsync($"**{serverName}** cannot be renamed: **{e.Message}**");
            }
        }
    }
}
