using DataStorage.DataObjects;
using DataStorage.DataObjects.Enums;
using DataStorage.Implementations.SQLite.SQLiteEngine;
using DataStorage.Interfaces;

namespace DataStorage.Implementations.SQLite
{
    internal class ServerParkEventRegister : BaseSQLiteController, IServerParkEventRegister
    {
        public async Task CreateServer(ulong serverId, string serverName, UserEventData userEventData)
        {
            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server (server_id, disk_size) VALUES (@serverId, 0);";
            cmd.Parameters.AddWithValue("@serverId", serverId);

            await cmd.ExecuteNonQueryAsync();

            await RenameServer(serverId, serverName, userEventData);
        }

        public Task DeleteServer(ulong serverId, UserEventData userEventData) =>
            RenameServer(serverId, null, userEventData);

        public async Task RenameServer(ulong serverId, string? newName, UserEventData userEventData)
        {
            ulong eventId = await CreateUserEvent(userEventData.Id, userEventData.Platform, UserEventType.ServerNameChange);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server_name(event_id, server_id, server_name) VALUES (@eventId, @serverId, @serverName);";
            cmd.Parameters.AddWithValue("@eventId", eventId);
            cmd.Parameters.AddWithValue("@serverId", serverId);
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
            ulong eventId = await CreateUserEvent(userEventData.Id, userEventData.Platform, UserEventType.ServerStatusChange);

            using var conn = CreateOpenConnection;

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO minecraft_server_status(event_id, server_id, status_id) VALUES (@eventId, @serverId, @statusId);";
            cmd.Parameters.AddWithValue("@eventId", eventId);
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@statusId", (int)status);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
