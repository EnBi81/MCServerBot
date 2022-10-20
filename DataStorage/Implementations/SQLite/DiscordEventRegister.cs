using DataStorage.DataObjects;
using DataStorage.DataObjects.Enums;
using DataStorage.Implementations.SQLite.SQLiteEngine;
using DataStorage.Interfaces;

namespace DataStorage.Implementations.SQLite
{
    internal class DiscordEventRegister : BaseSQLiteController, IDiscordEventRegister
    {
        public async Task<DataUser?> GetUser(ulong id)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT user_id, username, profile_pic_url, web_access_token FROM discord_user WHERE user_id = @userId;";
            cmd.Parameters.AddWithValue("@userId", id);

            using var reader = await cmd.ExecuteReaderAsync();
            
            if(!reader.HasRows)
                return null;

            DataUser user = new ()
            {
                Id = (ulong)reader.GetInt64(0),
                Username = reader.GetString(1),
                ProfilePicUrl = reader.GetString(2),
                WebAccessToken = reader.GetString(3),
            };

            return user;
        }


        public async Task RevokePermission(ulong userId, ulong discordId) =>
            await InsertIntoAccessTracker(userId, discordId, AccessEventType.Revoked);


        public async Task<bool> HasPermission(ulong id)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT type FROM access_tracker WHERE affected_id = @userId ORDER BY event_id DESC LIMIT 1;";
            cmd.Parameters.AddWithValue("@userId", id);

            object? res = await cmd.ExecuteScalarAsync();

            return res is int accessCode && accessCode == (int)AccessEventType.Granted;
        }


        public async Task RefreshUser(ulong id, string username, string profilePicUrl)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE discord_user SET username = @username, profile_pic_url = @prof_pic WHERE user_id = @userId;";
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@prof_pic", profilePicUrl);
            cmd.Parameters.AddWithValue("@userId", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RegisterDiscordUser(ulong discordId, string username, string profilepic, string webAccessToken)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO discord_user(user_id, username, profile_pic_url, web_access_token) VALUES (@user_id, @username, @prof_pic, @webaccesstoken);";
            cmd.Parameters.AddWithValue("@user_id", discordId);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@prof_pic", profilepic);
            cmd.Parameters.AddWithValue("@webaccesstoken", webAccessToken);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task GrantPermission(ulong userId, ulong discordId) =>
            await InsertIntoAccessTracker(userId, discordId, AccessEventType.Granted);


        private async Task InsertIntoAccessTracker(ulong userId, ulong discordId, AccessEventType type)
        {
            var userEventId = await CreateUserEvent(userId, Platform.Discord, UserEventType.AccessModified);

            using var conn = CreateOpenConnection;
            var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO access_tracker(event_id, affected_id, type) VALUES (@eventId, @affectedId, @accessType);";
            cmd.Parameters.AddWithValue("@eventId", userEventId);
            cmd.Parameters.AddWithValue("@affectedId", discordId);
            cmd.Parameters.AddWithValue("@accessType", (int)type);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
