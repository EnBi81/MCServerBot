using Shared.Model;

namespace MCServerDotNet.API.Internal.ModelImplementations
{
    internal class MinecraftServerStub : IMinecraftServer
    {
        public ulong Id { get; internal set; }

        public string ServerName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ServerStatus Status { get; internal set; }

        public bool IsRunning { get; internal set; }

        public List<ILogMessage> Logs { get; internal set; }

        public DateTime? OnlineFrom { get; internal set; }

        public IMinecraftServerProperties Properties { get; internal set; }

        public int Port { get; internal set; }

        public Dictionary<string, IMinecraftPlayer> Players { get; internal set; }

        public string StorageSpace { get; internal set; }

        public long StorageBytes { get; internal set; }

        public event EventHandler<ServerStatus> StatusChange;
        public event EventHandler<ILogMessage> LogReceived;
        public event EventHandler<IMinecraftPlayer> PlayerJoined;
        public event EventHandler<IMinecraftPlayer> PlayerLeft;
        public event EventHandler<(string CPU, string Memory)> PerformanceMeasured;
        public event EventHandler<string> NameChanged;

        public void Shutdown(string user = "Admin")
        {
            throw new NotImplementedException();
        }

        public void Start(string user = "Admin")
        {
            throw new NotImplementedException();
        }

        public void WriteCommand(string command, string user = "Admin")
        {
            throw new NotImplementedException();
        }
    }
}
