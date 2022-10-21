using DataStorage.DataObjects;
using DataStorage.DataObjects.Enums;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Implementations.SQLite.SQLiteEngine
{
    internal class BaseSQLiteController
    {
        internal static string? _connectionString;
        internal static string ConnectionString { get => _connectionString ?? throw new NullReferenceException("Connection String must be set!"); set => _connectionString = value; }
        public SQLiteConnection CreateOpenConnection => new SQLiteConnection(ConnectionString).OpenAndReturn();
        public string CurrentDateTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        public async Task<long> CreateUserEvent(ulong userId, Platform platform, UserEventType eventType)
        {
            using var conn = CreateOpenConnection;
            var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO user_event(user_id, platform_id, event_type_id, time_at) VALUES (@userId, @platformId, @eventTypeId, @timeAt);";
            cmd.Parameters.AddWithValue("@userId", userId.ToString());
            cmd.Parameters.AddWithValue("@platformId", (int)platform);
            cmd.Parameters.AddWithValue("@eventTypeId", (int)eventType);
            cmd.Parameters.AddWithValue("@timeAt", CurrentDateTime);

            await cmd.ExecuteNonQueryAsync();

            return conn.LastInsertRowId;
        }


        public async Task<DataUser?> GetUserFromCommand(SQLiteCommand cmd)
        {
            using var reader = await cmd.ExecuteReaderAsync();

            if (!reader.HasRows)
                return null;

            await reader.ReadAsync();
            DataUser user = new()
            {
                Id = ulong.Parse(reader.GetString(0)),
                Username = reader.GetString(1),
                ProfilePicUrl = reader.GetString(2),
                WebAccessToken = reader.GetString(3),
            };

            return user;
        }
    }
}
