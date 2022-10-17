using Discord.Interactions;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Minecraft;
using Application.Minecraft.MinecraftServers;

namespace DiscordBot.Bot.Commands.Autocompletes
{
    public class ServerNameAutocomplete : AutocompleteHandler
    {
        private IServerPark _serverPark;

        public ServerNameAutocomplete(IServerPark serverPark)
        {
            _serverPark = serverPark;
        }


        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
        {
            string currentValue = autocompleteInteraction.Data.Current.Value.ToString()!.ToLower();

            IEnumerable<IMinecraftServer> servers = _serverPark.MCServers.Values;

            IEnumerable<AutocompleteResult> results = 
                from server in servers 
                let serverNameLower = server.ServerName.ToLower()
                where serverNameLower.StartsWith(currentValue)
                select new AutocompleteResult(server.ServerName, server.ServerName);


            // max - 25 suggestions at a time (API limit)
            var result = AutocompletionResult.FromSuccess(results.Take(25));
            return Task.FromResult(result);
        }
    }
}
