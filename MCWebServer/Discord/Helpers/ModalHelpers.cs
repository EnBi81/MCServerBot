using Discord;

namespace MCWebServer.Discord.Helpers
{
    public class ModalHelpers
    {
        public const string RenameServerModalId = "rename-server-modal";
        public const string DeleteServerModalId = "delete-server-modal";

        public static ModalBuilder RenameServerBuilder()
        {
            TextInputBuilder textInputB = new TextInputBuilder()
                .WithCustomId("new-name")
                .WithRequired(true)
                .WithStyle(TextInputStyle.Short)
                .WithMaxLength(MinecraftServer.MinecraftServer.NAME_MAX_LENGTH)
                .WithMinLength(MinecraftServer.MinecraftServer.NAME_MIN_LENGTH)
                .WithLabel("New name of the server");

            //SelectMenuBuilder selectMenu = MenuHelpers.ServerListMenuBuilder("selected-server");

            ModalBuilder builder = new ModalBuilder()
                .WithCustomId(RenameServerModalId)
                .WithTitle($"Rename a Servers")
               //.AddComponents(new() { selectMenu.Build() }, 1)
                .AddComponents(new() { textInputB.Build() }, 2);

            return builder;
        }

        public static ModalBuilder DeleteServerBuilder(string serverName)
        {
            TextInputBuilder textInputB = new TextInputBuilder()
                .WithCustomId("server-name")
                .WithRequired(true)
                .WithStyle(TextInputStyle.Short)
                .WithMaxLength(serverName.Length)
                .WithMinLength(serverName.Length)
                .WithLabel("Please enter the name of the server again")
                .WithPlaceholder(serverName);

            //SelectMenuBuilder selectMenu = MenuHelpers.ServerListMenuBuilder("selected-server");

            ModalBuilder builder = new ModalBuilder()
                .WithCustomId(DeleteServerModalId)
                .WithTitle("Delete " + serverName)
                .AddTextInput(textInputB);
                //.AddComponents(new() { selectMenu.Build() }, 1)

            return builder;
        }
    }
}
