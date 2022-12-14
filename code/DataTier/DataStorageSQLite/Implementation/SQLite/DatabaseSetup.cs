using Application.DAOs.Database;
using DataStorageSQLite.Implementations.SQLite.SQLiteEngine;
using System.Data.SQLite;

namespace DataStorageSQLite.Implementations.SQLite
{
    internal class DatabaseSetup : BaseSQLiteController, IDatabaseSetup
    {
        public async Task Setup(string connectionString) // Data Source=c:\mydb.db;Version=3;
        {
            // assign connection string to global handler
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

            // get the path of the sqlite database file
            string path = connectionString[(connectionString.IndexOf("Data Source=") + "Data Source=".Length)..];
            path = path[..path.IndexOf(";")];

            // check if file exists
            if (File.Exists(path))
                return;
            
            // create an empty database file
            SQLiteConnection.CreateFile(path);

            await ResetDatabase();
        }


        public async Task ResetDatabase()
        {
            using SQLiteConnection conn = CreateOpenConnection;

            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = SetupString;

            await cmd.ExecuteNonQueryAsync();
        }


        /// <summary>
        /// SQL strings that sets up the database from the start. 
        /// </summary>
        public static string SetupString => "DROP TABLE IF EXISTS platform; CREATE TABLE platform ( platform_id INTEGER PRIMARY KEY AUTOINCREMENT, platform_name TEXT ); INSERT INTO platform(platform_name) VALUES ('Discord'), ('Website'); DROP TABLE IF EXISTS event_type; CREATE TABLE event_type ( type_id INTEGER PRIMARY KEY AUTOINCREMENT, type_name TEXT ); INSERT INTO event_type(type_name) VALUES ('server-name-change'), ('command'), ('server-status-change'), ('access-modified'); DROP TABLE IF EXISTS discord_user; CREATE TABLE discord_user ( user_id TEXT PRIMARY KEY, username TEXT, profile_pic_url TEXT, web_access_token TEXT ); INSERT INTO discord_user(user_id, username, profile_pic_url) VALUES (1, 'System', NULL); DROP TABLE IF EXISTS user_event; CREATE TABLE user_event ( event_id INTEGER PRIMARY KEY AUTOINCREMENT, user_id TEXT REFERENCES discord_user (user_id), platform_id INTEGER REFERENCES platform (platform_id), event_type_id INTEGER REFERENCES event_type (type_id), time_at TEXT ); DROP TABLE IF EXISTS access_type; CREATE TABLE access_type ( access_id INTEGER PRIMARY KEY AUTOINCREMENT, access_name TEXT ); INSERT INTO access_type(access_name) VALUES ('granted'), ('revoked'); DROP TABLE IF EXISTS access_tracker; CREATE TABLE access_tracker ( event_id INTEGER REFERENCES user_event (event_id), affected_id TEXT REFERENCES discord_user (user_id), type INTEGER REFERENCES access_type (access_id) ); DROP TABLE IF EXISTS minecraft_server; CREATE TABLE minecraft_server ( server_id TEXT PRIMARY KEY, disk_size INTEGER ); DROP TABLE IF EXISTS minecraft_server_name; CREATE TABLE minecraft_server_name ( event_id INTEGER REFERENCES user_event (event_id), server_id TEXT REFERENCES minecraft_server (server_id), server_name TEXT ); DROP TABLE IF EXISTS minecraft_server_command; CREATE TABLE minecraft_server_command ( event_id INTEGER REFERENCES user_event (event_id), server_id TEXT REFERENCES minecraft_server (server_id), command TEXT ); DROP TABLE IF EXISTS server_status_type; CREATE TABLE server_status_type ( status_id INTEGER PRIMARY KEY AUTOINCREMENT, status_name TEXT ); INSERT INTO server_status_type(status_name) VALUES ('Start'), ('Stop'); DROP TABLE IF EXISTS minecraft_server_status; CREATE TABLE minecraft_server_status ( event_id INTEGER REFERENCES user_event (event_id), server_id TEXT REFERENCES minecraft_server (server_id), status_id INTEGER REFERENCES server_status_type (status_id) ); DROP TABLE IF EXISTS measurements; CREATE TABLE measurements ( server_id TEXT REFERENCES minecraft_server (server_id), time_at TEXT, cpu INTEGER, memory INTEGER ); DROP TABLE IF EXISTS minecraft_player; CREATE TABLE minecraft_player ( player_id INTEGER PRIMARY KEY AUTOINCREMENT, username TEXT ); DROP TABLE IF EXISTS mc_event_type; CREATE TABLE mc_event_type ( type_id INTEGER PRIMARY KEY AUTOINCREMENT, type_name TEXT ); INSERT INTO mc_event_type(type_name) VALUES ('joined'), ('left'); DROP TABLE IF EXISTS mc_player_event; CREATE TABLE mc_player_event ( server_id TEXT REFERENCES minecraft_server (server_id), player_id INTEGER REFERENCES minecraft_player (player_id), time_at TEXT, event_type INTEGER REFERENCES mc_event_type (type_id) );";
    }
}
