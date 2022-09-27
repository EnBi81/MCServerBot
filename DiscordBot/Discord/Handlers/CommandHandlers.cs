using Discord.WebSocket;
using DiscordBot.Discord.Commands;

namespace DiscordBot.Discord.Handlers
{
    public class CommandHandlers
    {
        public static IReadOnlyDictionary<string, Func<SocketSlashCommand, Task>> Commands { get; } =
            new Dictionary<string, Func<SocketSlashCommand, Task>>()
            {
                ["start-server"] = MinecraftServerCommands.StartServer,
                ["stop-server"] = MinecraftServerCommands.ShutDownServer,
                ["grant-perm"] = PermissionCommands.GrantPermission,
                ["revoke-perm"] = PermissionCommands.RevokePermission,
                ["get-web-url"] = PermissionCommands.GetWebLoginPage,
                ["ping"] = ToolCommands.Ping,
                ["reset-commands"] = ToolCommands.ResetAllCommands,
                ["create-server"] = MinecraftServerCommands.CreateServer,
                ["rename-server"] = MinecraftServerCommands.RenameServer,
                ["delete-server"] = MinecraftServerCommands.DeleteServer
            };

        public static async Task HandleCommand(SocketSlashCommand arg)
        {
            if(!Commands.TryGetValue(arg.Data.Name, out Func<SocketSlashCommand, Task> function))
            {
                await arg.RespondAsync("Unrecognized command", ephemeral: true);
                return;
            }

            // execute command
            await function(arg);
        }
    }
}
