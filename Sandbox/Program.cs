using System;
using System.Diagnostics;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            var sqLite = DataStorage.DatabaseAccess.SQLite;
            await sqLite.Setup("Data Source=C:\\Users\\enbi8\\source\\repos\\MCServerBot\\Sandbox\\mydb.db;Version=3;");

            var discordStorage = sqLite.DiscordEventRegister;


            await discordStorage.RefreshUser(ulong.MaxValue, "refreshedUsername", "refreshedProfPic");
            var user = await discordStorage.GetUser(ulong.MaxValue);
            Console.WriteLine("user: " + user);
        }


        static async Task TestUser()
        {
            var sqLite = DataStorage.DatabaseAccess.SQLite;
            var discordStorage = sqLite.DiscordEventRegister;

            //await discordStorage.RegisterDiscordUser(ulong.MaxValue, "Minimillian", "profpic", "haha");
            var user = await discordStorage.GetUser(ulong.MaxValue);

            Console.WriteLine("Has permission first: " + await discordStorage.HasPermission(ulong.MaxValue));
            await discordStorage.GrantPermission(ulong.MaxValue, ulong.MaxValue);
            Console.WriteLine("Has permission true: " + await discordStorage.HasPermission(ulong.MaxValue));
            await discordStorage.RevokePermission(ulong.MaxValue, ulong.MaxValue);
            Console.WriteLine("Has permission false: " + await discordStorage.HasPermission(ulong.MaxValue));
            Console.WriteLine("user: " + user);
        }
    }
}

