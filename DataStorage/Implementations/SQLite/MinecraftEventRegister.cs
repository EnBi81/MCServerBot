using DataStorage.DataObjects;
using DataStorage.DataObjects.Enums;
using DataStorage.Implementations.SQLite.SQLiteEngine;
using DataStorage.Interfaces;

namespace DataStorage.Implementations.SQLite
{
    internal class MinecraftEventRegister : BaseSQLiteController, IMinecraftEventRegister
    {
        public void AddMeasurement(ulong serverId, double cpu, long memory)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO measurements(server_id, time_at, cpu, memory) VALUES (@serverId, @timeAt, @cpu, @memory);";
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@timeAt", CurrentDateTime);
            cmd.Parameters.AddWithValue("@cpu", cpu);
            cmd.Parameters.AddWithValue("@memory", memory);

            cmd.ExecuteNonQuery();
        }

        public void PlayerJoined(ulong serverId, string username) =>
            InsertIntoMcPlayerEvent(serverId, username, PlayerEventType.Joined);

        public void PlayerLeft(ulong serverId, string username) =>
            InsertIntoMcPlayerEvent(serverId, username, PlayerEventType.Left);

        private void InsertIntoMcPlayerEvent(ulong serverId, string username, PlayerEventType eventType)
        {
            long playerId = GetPlayerId(username);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO mc_player_event(server_id, player_id, time_at, event_type) VALUES (@serverId, @playerId, @timeAt, @eventType);";
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@playerId", playerId);
            cmd.Parameters.AddWithValue("@timeAt", CurrentDateTime);
            cmd.Parameters.AddWithValue("@eventType", (int)eventType);

            cmd.ExecuteNonQuery();
        }

        private long GetPlayerId(string username)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT player_id FROM minecraft_player WHERE username = @username;";
            cmd.Parameters.AddWithValue("@username", username);

            object? result = cmd.ExecuteScalar();

            if (result is long int64)
                return int64;
            if (result is int int32)
                return int32;


            cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_player(username) VALUES (@username);";
            cmd.Parameters.AddWithValue("@username", username);

            cmd.ExecuteNonQuery();

            return conn.LastInsertRowId;
        }

        public void SetDiskSize(ulong serverId, long diskSize)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE minecraft_server SET disk_size = @diskSize WHERE server_id = @serverId;";
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@diskSize", diskSize);

            cmd.ExecuteNonQuery();
        }
    }
}
