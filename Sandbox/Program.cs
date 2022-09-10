using System;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            var SocketClient = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                LogLevel = LogSeverity.Debug,

            });

            var CmdService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Debug,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,

            });

            var Services = SetupServices(SocketClient, CmdService);

            var CommandHandler = new CommandHandler(SocketClient);

            await CmdService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            await SocketClient.LoginAsync(TokenType.Bot, "NjkyMDczMTUyNjE1ODA5MDg1.GJfn9f.46z2sX3jwywlWi2SqHFs5_9U1UIotCCHayqMnE");
            await SocketClient.StartAsync();
            await CommandHandler.InitializeAsync();

            await SocketClient.SetGameAsync("Server Offline", null, ActivityType.Playing);

            await Task.Delay(-1);
        }

        private static IServiceProvider SetupServices(params object[] objects)
            => new ServiceCollection()
            .AddSingleton(objects[0])
            .AddSingleton(objects[1])
            .BuildServiceProvider();
    }
}

