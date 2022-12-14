using Application.DAOs.Database;
using Application.Minecraft;
using DataStorage.Interfaces;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Bot.Handlers.Autocompletes;
using DiscordBot.Bot.Helpers;

namespace DiscordBot.Bot.Handlers
{
    public class DeleteServerCommands : MCInteractionModuleBase
    {

        private readonly IServerPark _serverPark;
        private readonly DeleteServerService _deleteServerService;

        public DeleteServerCommands(IServerPark serverPark, DeleteServerService deleteServerService, IDiscordDataAccess eventRegister) : base(eventRegister)
        {
            _serverPark = serverPark;
            _deleteServerService = deleteServerService;
        }


        [SlashCommand("delete-server", "Delete a server")]
        public async Task DeleteServer([Summary("server-name", "Name of the server to delete"), Autocomplete(typeof(ServerNameAutocomplete))] string serverName)
        {
            if(!_serverPark.MCServers.TryGetValue(serverName, out var server))
            {
                await RespondAsync($"Couldn't find server **{serverName}**");
                return;
            }
            

            var cancelButton = ButtonHelper.CreateCancelButton("delete-cancel");
            var proceedButton = ButtonHelper.CreateProceedButton("delete-proceed", "Delete");

            var messageComponent = ButtonHelper.JoinButtons(cancelButton, proceedButton);

            IUserMessage replyMessage = await ReplyAsync($"Do you really want to delete **{serverName}**?", components: messageComponent);
            _deleteServerService.Add(replyMessage, server);
        }


        [ComponentInteraction("delete-cancel")]
        public async Task DeleteCancel()
        {
            if (Context.Interaction is not SocketMessageComponent messageComponent)
            {
                await RespondAsync("Something went reaaaaaaaaally wrong!");
                return;
            }

            var message = messageComponent.Message;
            var deleteObj = _deleteServerService.Remove(message.Id);

            await message.ModifyAsync(prop =>
            {
                prop.Content = $"Deleting **{deleteObj?.MinecraftServer?.ServerName}** was **cancelled**.";
                prop.Components = null;
            });
        }


        [ComponentInteraction("delete-proceed")]
        public async Task DeleteProceed()
        {
            if (Context.Interaction is not SocketMessageComponent messageComponent)
            {
                await RespondAsync("Something went reaaaaaaaaally wrong!");
                return;
            }

            var message = messageComponent.Message;
            var deleteObj = _deleteServerService.Remove(message.Id);

            if(deleteObj == null)
            {
                await RespondAsync("Something went wrong :((");
                return;
            }

            string serverName = deleteObj.MinecraftServer.ServerName;

            var user = await GetUserEventData();
            await _serverPark.DeleteServer(serverName, user);

            await message.ModifyAsync(prop => 
            {
                prop.Content = $"**{serverName}** is **deleted**.";
                prop.Components = null;
            });
        }
    }
}
