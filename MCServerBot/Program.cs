using MCWebServer.Discord;
using MCWebServer.Hamachi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MCWebServer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main(string[] args)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-6.0

            //await SetupDiscordBot();
            SetupGUI();
        }

        public static async Task SetupDiscordBot()
        {
            DiscordBot bot = new DiscordBot();
            await bot.InitializeAsync();
        }

        public static void SetupGUI()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
