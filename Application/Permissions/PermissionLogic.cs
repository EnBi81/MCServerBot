﻿using Application.DAOs;
using Application.DAOs.Database;
using Shared.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace Application.Permissions
{
    public class PermissionLogic : IPermissionLogic
    {
        private readonly Dictionary<string, DataUser> BuiltInUsers = new Dictionary<string, DataUser>
        {
            ["RWodc/j=XxPUBq^(Yoxim\":b~#jl~6RdEAk:W[m]ad06g.v1UA<`hsMo(1n>MSj"] = new DataUser
            {
                Id = 2, 
                Username = "DCBot",
                ProfilePicUrl = "",
                UserType = DataUserType.Discord
            }
        };

        private readonly IPermissionDataAccess _permissionAccess;

        public PermissionLogic(IDatabaseAccess databaseAccess)
        {
            _permissionAccess = databaseAccess.PermissionDataAccess;
        }


        public async Task<string> GetWebAccessCode(ulong user)
        {
            return await _permissionAccess.GetWebAccessCode(user) ?? throw new Exception("User is not registered in the system.");
        }

        public async Task<string> GetToken(ulong id) =>
            await _permissionAccess.GetWebAccessCode(id) ?? 
            throw new Exception("User is not registered in the system.");

        public async Task<DataUser> GetUser(string token)
        {
            if (BuiltInUsers.TryGetValue(token, out DataUser? user))
                return user;

            return await _permissionAccess.GetUser(token) ?? throw new Exception("User is not registered in the system.");
        }

        public async Task<bool> HasAccess(string token)
        {
            if (BuiltInUsers.ContainsKey(token))
                return true;

            return await _permissionAccess.HasPermission(token);
        }

        public async Task RegisterUser(ulong discordId, string? discordUsername, string? profPic)
        {
            if (discordUsername is null)
                throw new ArgumentNullException(nameof(discordUsername));

            if (profPic is null)
                throw new ArgumentNullException(nameof(profPic));

            string webAccessCode = GetHashString(DateTime.Now.ToString("G") + discordId.ToString());
            await _permissionAccess.RegisterDiscordUser(discordId, discordUsername, profPic, webAccessCode);
        }


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
