using Application.DAOs.Database;
using DataStorageSQLite.Implementations.SQLite.SQLiteEngine;
using SharedPublic.DTOs;
using SharedPublic.DTOs.Enums;

namespace DataStorageSQLite.Implementations.SQLite
{
    internal class MinecraftDAO : BaseSQLiteController, IMinecraftDataAccess
    {
        public void AddMeasurement(long serverId, double cpu, long memory)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO measurements(server_id, time_at, cpu, memory) VALUES (@serverId, @timeAt, @cpu, @memory);";
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());
            cmd.Parameters.AddWithValue("@timeAt", CurrentDateTime);
            cmd.Parameters.AddWithValue("@cpu", cpu);
            cmd.Parameters.AddWithValue("@memory", memory);

            cmd.ExecuteNonQuery();
        }

        public void PlayerJoined(long serverId, string username) =>
            InsertIntoMcPlayerEvent(serverId, username, PlayerEventType.Joined);

        public void PlayerLeft(long serverId, string username) =>
            InsertIntoMcPlayerEvent(serverId, username, PlayerEventType.Left);

        private void InsertIntoMcPlayerEvent(long serverId, string username, PlayerEventType eventType)
        {
            long playerId = GetPlayerId(username);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO mc_player_event(server_id, player_id, time_at, event_type) VALUES (@serverId, @playerId, @timeAt, @eventType);";
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());
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

        public void SetDiskSize(long serverId, long diskSize)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE minecraft_server SET disk_size = @diskSize WHERE server_id = @serverId;";
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());
            cmd.Parameters.AddWithValue("@diskSize", diskSize);

            cmd.ExecuteNonQuery();
        }

        public async void WriteCommand(long serverId, string command, UserEventData userEventData)
        {
            var eventId = await CreateUserEvent(userEventData.Id, userEventData.Platform, UserEventType.ServerStatusChange);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server_command(event_id, server_id, command) VALUES (@eventId, @serverId, @command);";
            cmd.Parameters.AddWithValue("@eventId", eventId);
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());
            cmd.Parameters.AddWithValue("@command", command);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
