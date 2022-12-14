using Application.DAOs.Database;
using DataStorageSQLite.Implementations.SQLite.SQLiteEngine;
using SharedPublic.DTOs;
using SharedPublic.DTOs.Enums;

namespace DataStorageSQLite.Implementations.SQLite
{
    internal class ServerParkDAO : BaseSQLiteController, IServerParkDataAccess
    {
        /// <inheritdoc/>
        public async Task<string?> GetServerName(long serverId)
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

        /// <inheritdoc/>
        public async Task CreateServer(long serverId, string serverName, UserEventData userEventData)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server (server_id, disk_size) VALUES (@serverId, 0);";
            cmd.Parameters.AddWithValue("@serverId", serverId.ToString());

            await cmd.ExecuteNonQueryAsync();

            await RenameServer(serverId, serverName, userEventData);
        }

        /// <inheritdoc/>
        public Task DeleteServer(long serverId, UserEventData userEventData) =>
            RenameServer(serverId, null, userEventData);

        /// <inheritdoc/>
        public async Task RenameServer(long serverId, string? newName, UserEventData userEventData)
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

        /// <inheritdoc/>
        public async Task<long> GetMaxServerId()
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT server_id FROM minecraft_server ORDER BY server_id DESC LIMIT 1;";

            object? res = await cmd.ExecuteScalarAsync();

            return long.TryParse(res?.ToString(), out long value) ? value : 0;
        }

        /// <inheritdoc/>
        public async Task StartServer(long serverId, UserEventData userEventData) =>
            await InsertIntoServerStatus(serverId, ServerStatus.Start, userEventData);

        /// <inheritdoc/>
        public async Task StopServer(long serverId, UserEventData userEventData) =>
            await InsertIntoServerStatus(serverId, ServerStatus.Stop, userEventData);

        /// <summary>
        /// Inserts a record into the server status change table
        /// </summary>
        /// <param name="serverId">id of the server.</param>
        /// <param name="status">status of the server-</param>
        /// <param name="userEventData">user event data.</param>
        /// <returns></returns>
        private async Task InsertIntoServerStatus(long serverId, ServerStatus status, UserEventData userEventData)
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
