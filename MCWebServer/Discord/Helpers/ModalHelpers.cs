using Discord;

namespace MCWebServer.Discord.Helpers
{
    public class ModalHelpers
    {
        public const string RenameServerModalId = "rename-server-modal";
        public const string DeleteServerModalId = "delete-server-modal";


        public static ModalBuilder DeleteServerBuilder(string serverName)
        {
            TextInputBuilder textInputB = NameSpecificTextBuilder(serverName);

            ModalBuilder builder = new ModalBuilder()
                .WithCustomId(DeleteServerModalId)
                .WithTitle("Delete " + serverName)
                .AddTextInput(textInputB);

            return builder;
        }

        public static ModalBuilder RenameServerBuilder(string serverName)
        {
            TextInputBuilder textInputB = NameSpecificTextBuilder(serverName);

            ModalBuilder builder = new ModalBuilder()
                .WithCustomId(RenameServerModalId)
                .WithTitle("Rename " + serverName)
                .AddTextInput(textInputB)
                .AddTextInput(label: "Please enter the new name of the server", 
                              customId: "new-server-name", 
                              placeholder: "New Server Name",
                              minLength: MinecraftServer.MinecraftServer.NAME_MIN_LENGTH, 
                              maxLength: MinecraftServer.MinecraftServer.NAME_MAX_LENGTH,
                              required: true);

            return builder;
        }

        private static TextInputBuilder NameSpecificTextBuilder(string name) => 
            new TextInputBuilder()
                .WithCustomId("server-name")
                .WithRequired(true)
                .WithStyle(TextInputStyle.Short)
                .WithMaxLength(name.Length)
                .WithMinLength(name.Length)
                .WithLabel("Please enter the name of the server again")
                .WithPlaceholder(name);
    }
}
