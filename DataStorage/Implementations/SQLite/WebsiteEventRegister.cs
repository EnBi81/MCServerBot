using DataStorage.DataObjects;
using DataStorage.DataObjects.Enums;
using DataStorage.Implementations.SQLite.SQLiteEngine;
using DataStorage.Interfaces;
using System.Text;

namespace DataStorage.Implementations.SQLite
{
    internal class WebsiteEventRegister : BaseSQLiteController, IWebsiteEventRegister
    {
        public async Task<DataUser?> GetUser(string token)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT user_id, username, profile_pic_url, web_access_token FROM discord_user WHERE web_access_token = @token;";
            cmd.Parameters.AddWithValue("@token", token);

            return await GetUserFromCommand(cmd);
        }

        public async Task<bool> HasPermission(string token)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT type FROM access_tracker at JOIN discord_user du ON du.user_id = at.affected_id " +
                              "WHERE web_access_token = @token ORDER BY event_id DESC LIMIT 1;";

            cmd.Parameters.AddWithValue("@token", token);

            object? res = await cmd.ExecuteScalarAsync();

            return res is long accessCode && accessCode == (int)AccessEventType.Granted;
        }
    }
}
