using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStorage.Implementations.SQLite
{
    internal class WebsiteEventRegister : IWebsiteEventRegister
    {
        public void AddCommand(string token, string command)
        {
            //SQLiteConnection.CreateFile("");
            //SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            //m_dbConnection.Open();

            //string sql = "create table highscores (name varchar(20), score int)";

            //SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            //command.Exe
            //command.ExecuteNonQuery();

            //sql = "insert into highscores (name, score) values ('Me', 9001)";

            //command = new SQLiteCommand(sql, m_dbConnection);
            //command.ExecuteNonQuery();

            //m_dbConnection.Close();
            throw new NotImplementedException();
        }

        public void CreateServer(string token, string serverName, long storageSpace)
        {
            throw new NotImplementedException();
        }

        public void DeleteServer(string token, ulong serverId)
        {
            throw new NotImplementedException();
        }

        public void RenameServer(string token, ulong serverId, string newName)
        {
            throw new NotImplementedException();
        }

        public void StartServer(string token, ulong serverId)
        {
            throw new NotImplementedException();
        }

        public void StopServer(string token, ulong serverId)
        {
            throw new NotImplementedException();
        }
    }
}
