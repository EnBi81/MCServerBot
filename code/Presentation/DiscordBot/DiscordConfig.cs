namespace DiscordBot
{
    public class DiscordConfig
    {
        private static DiscordConfig? _instance;
        public static DiscordConfig Instance { get => _instance ?? throw new Exception("Discord Config is not set!"); }

        public static void SetupConfig(string discordBotToken, int websiteHttpsPort)
        {
            if (_instance != null)
                return;

            _instance = new DiscordConfig()
            {
                DiscordBotToken = discordBotToken,
                WebsiteHttpsPort = websiteHttpsPort,
            };
        }


        private DiscordConfig() { }

        public string DiscordBotToken { get; private set; } = "";
        public int WebsiteHttpsPort { get; private set; }
    }
}
