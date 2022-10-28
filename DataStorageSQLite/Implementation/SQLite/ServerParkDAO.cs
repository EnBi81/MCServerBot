using Application.DAOs.Database;
using DataStorageSQLite.Implementations.SQLite.SQLiteEngine;
using Shared.DTOs;
using Shared.DTOs.Enums;

namespace DataStorageSQLite.Implementations.SQLite
{
    internal class ServerParkDAO : BaseSQLiteController, IServerParkDataAccess
    {
        public async Task<string?> GetServerName(ulong serverId)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "WITH data AS (SELECT ms.server_id, msn.server_name, event_id " +
                "FROM minecraft_server ms JOIN minecraft_server_name msn ON ms.server_id = msn.server_id ORDER BY event_id DESC) " +
                "SELECT server_name FROM data WHERE server_id = @serverId GROUP BY server_id HAVING server_name NOT NULL;";
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());

            object? result = await cmd.ExecuteScalarAsync();

            return result is string s ? s : null;
        }

        public async Task CreateServer(ulong serverId, string serverName, UserEventData userEventData)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server (server_id, disk_size) VALUES (@serverId, 0);";
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());

            await cmd.ExecuteNonQueryAsync();

            await RenameServer(serverId, serverName, userEventData);
        }

        public Task DeleteServer(ulong serverId, UserEventData userEventData) =>
            RenameServer(serverId, null, userEventData);

        public async Task RenameServer(ulong serverId, string? newName, UserEventData userEventData)
        {
            var eventId = await CreateUserEvent(userEventData.Id, userEventData.Platform, UserEventType.ServerNameChange);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server_name(event_id, server_id, server_name) VALUES (@eventId, @serverId, @serverName);";
            cmd.Parameters.AddWithValue("@eventId", eventId);
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());
            cmd.Parameters.AddWithValue("@serverName", newName);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<ulong> GetMaxServerId()
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT server_id FROM minecraft_server ORDER BY server_id DESC LIMIT 1;";

            object? res = await cmd.ExecuteScalarAsync();

            return res is ulong num ? num : 0;
        }

        public async Task StartServer(ulong serverId, UserEventData userEventData) =>
            await InsertIntoServerStatus(serverId, ServerStatus.Start, userEventData);

        public async Task StopServer(ulong serverId, UserEventData userEventData) =>
            await InsertIntoServerStatus(serverId, ServerStatus.Stop, userEventData);

        private async Task InsertIntoServerStatus(ulong serverId, ServerStatus status, UserEventData userEventData)
        {
            var eventId = await CreateUserEvent(userEventData.Id, userEventData.Platform, UserEventType.ServerStatusChange);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server_status(event_id, server_id, status_id) VALUES (@eventId, @serverId, @statusId);";
            cmd.Parameters.AddWithValue("@eventId", eventId);
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());
            cmd.Parameters.AddWithValue("@statusId", (int)status);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
