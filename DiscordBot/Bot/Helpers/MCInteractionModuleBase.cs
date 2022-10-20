﻿using DataStorage.DataObjects;
using DataStorage.Interfaces;
using Discord.Interactions;

namespace DiscordBot.Bot.Helpers
{
    public class MCInteractionModuleBase : InteractionModuleBase<SocketInteractionContext>
    {
        protected readonly IDiscordEventRegister _eventRegister;

        public MCInteractionModuleBase(IDiscordEventRegister eventRegister)
        {
            _eventRegister = eventRegister;
        }


        public async Task<DataUser> GetUser()
        {
            return await _eventRegister.GetUser(Context.User.Id) ?? throw new Exception("No permission for this user.");
        }
    }
}