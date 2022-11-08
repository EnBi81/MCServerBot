using Application.Minecraft.MinecraftServers;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordBot.Bot.Helpers
{
    public class DeleteServerService
    {
        public Dictionary<ulong, DeleteServerObject> Collection { get; } = new ();




        public void Add(IUserMessage message, IMinecraftServer server)
        {
            var deleteServerObj = new DeleteServerObject()
            {
                Message = message,
                MinecraftServer = server,
                Created = DateTime.Now,
            };

            Collection.Add(message.Id, deleteServerObj);
        }

        public DeleteServerObject? Remove(ulong id)
        {
            Collection.Remove(id, out var deleteServerObj);
            deleteServerObj?.StopTimer();
            return deleteServerObj;
        }



        public class DeleteServerObject
        {
            public IUserMessage Message { get; set; } = null!;
            public IMinecraftServer MinecraftServer { get; set; } = null!;
            public DateTime Created { get; set; }


            private DeleteServerService? service;
            private System.Timers.Timer? timer;

            public DeleteServerObject()
            {
                timer = new()
                {
                    AutoReset = false,
                    Enabled = true,
                    Interval = 60 * 1000
                };
                timer.Elapsed += (_, _) => DeleteObject();
            }

            internal void StopTimer()
            {
                timer?.Dispose();
                timer = null;
            }

            public async void DeleteObject()
            {
                if (service?.Remove(Message?.Id ?? 0) != null)
                {
                    try
                    {
                        await Message.DeleteAsync();
                    }
                    catch { }
                }

                service = null;
                StopTimer();
            }
        }
    }
}
