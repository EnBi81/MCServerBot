using DataStorage.DataObjects;
using DataStorage.Interfaces;
using Discord;
using System.Security.Cryptography;
using System.Text;

namespace DiscordBot.Bot
{
    public class DiscordPermission
    {
        private readonly IDiscordEventRegister _discordEventRegister;
        private readonly CommandHandler _commandHandler;

        public DiscordPermission(IDiscordEventRegister discordEventRegister, CommandHandler cmdHandler)
        {
            _discordEventRegister = discordEventRegister;
            _commandHandler = cmdHandler;
        }



        public async Task<bool> HasPermission(ulong id)
        {
            if (id == _commandHandler.BotOwnerId)
                return true;

            return await _discordEventRegister.HasPermission(id);
        }



        public async Task<DataUser?> GetUser(ulong id) =>
            await _discordEventRegister.GetUser(id);

        public async Task GrantPermission(ulong grantedBy, IUser user)
        {
            ulong userId = user.Id;

            if (await GetUser(userId) == null)
            {
                string webCode = GetHashString(DateTime.Now.ToString("G") + userId.ToString());
                await _discordEventRegister.RegisterDiscordUser(userId, user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl(), webCode);
            }
                

            if (await _discordEventRegister.HasPermission(userId))
                throw new Exception(user.Username + " already has permission.");

            await _discordEventRegister.GrantPermission(grantedBy, userId);
        }
            
        public async Task RevokePermission(ulong revokedBy, IUser user)
        {
            if (user.Id == _commandHandler.BotOwnerId)
                throw new Exception("Cannot remove the owner of the bot.");

            if (!(await _discordEventRegister.HasPermission(user.Id)))
                throw new Exception(user.Username + " already hasn't got permission.");

            await _discordEventRegister.RevokePermission(revokedBy, user.Id);
        }
            
        public async Task RefreshUser(IUser user) => 
            await _discordEventRegister.RefreshUser(user.Id, user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl());


        /// <summary>
        /// Gets the hash of a string as a byte array.
        /// </summary>
        /// <param name="inputString">string to hash</param>
        /// <returns></returns>
        private static byte[] GetHash(string inputString)
        {
            using HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Gets the hash of a string as a string.
        /// </summary>
        /// <param name="inputString">string to hash</param>
        /// <returns></returns>
        private static string GetHashString(string inputString)
        {
            StringBuilder sb = new();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}
