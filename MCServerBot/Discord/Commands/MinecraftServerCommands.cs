using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCWebServer.Discord.Commands
{
    internal class MinecraftServerCommands
    {
        [Command("Start the minecraft server")]
        public static async Task StartServer(SocketSlashCommand command)
        {
            await command.RespondAsync("Starting Server...");
            try
            {
                MinecraftServer.ServerPark.Keklepcso.Start();
            }
            catch (Exception ex)
            {
                await command.Channel.SendMessageAsync("Starting failed: " + ex.Message);
            }
        }

        [Command("Stop the minecraft server")]
        public static async Task ShutDownServer(SocketSlashCommand command)
        {
            await command.RespondAsync("Shutting Down Server");

            try
            {
                MinecraftServer.ServerPark.Keklepcso.Shutdown();
            }
            catch (Exception ex)
            {
                await command.Channel.SendMessageAsync("Shutting Down failed: " + ex.Message);
            }
        }
    }
}
