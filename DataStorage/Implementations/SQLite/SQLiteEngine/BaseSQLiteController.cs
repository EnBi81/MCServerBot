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
        internal static string ConnectionString { get; set; } = null!;
        public SQLiteConnection CreateOpenConnection => new SQLiteConnection(ConnectionString).OpenAndReturn();
        public string CurrentDateTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        public async Task<ulong> CreateUserEvent(ulong userId, Platform platform, UserEventType eventType)
        {
            using var conn = CreateOpenConnection;
            var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO user_event(user_id, platform_id, event_type_id, time_at) VALUES (@userId, @platformId, @eventTypeId, @timeAt);";
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@platformId", (int)platform);
            cmd.Parameters.AddWithValue("@eventTypeId", (int)eventType);
            cmd.Parameters.AddWithValue("@timeAt", CurrentDateTime);

            await cmd.ExecuteNonQueryAsync();

            return (ulong)conn.LastInsertRowId;
        }
    }
}
