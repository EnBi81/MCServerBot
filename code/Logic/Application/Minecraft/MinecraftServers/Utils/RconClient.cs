using SharedPublic.Exceptions;
using SharedPublic.Model;
using System.Net;

namespace Application.Minecraft.MinecraftServers.Utils;

internal class RconClient : IDisposable
{
    /// <summary>
    /// Checks if the rcon is available for a minecraft server. THROWS EXCEPTION WITH THE ERROR MESSAGE IF NOT
    /// </summary>
    /// <param name="server"></param>
    /// <exception cref="MCExternalException">If the rcon is not available for the minecraft server.</exception>
    public static bool IsRconAvailable(IMinecraftServer server)
    {
        var serverVersion = server.MCVersion;

        // RCON is a protocol that allows server administrators to remotely execute Minecraft commands.
        // Introduced in Beta 1.9-pre4, it's basically an implementation of the Source RCON protocol for Minecraft.
        if (new Version(serverVersion.Version) < new Version("1.1"))
            throw new MCExternalException("RCON is not available for this minecraft server version. Please update to 1.1 or higher.");

        if (server.Properties["enable-rcon"] != "true") 
            throw new MCExternalException("RCON is not enabled for this minecraft server. To use RCON, enable it in the properties.");

        if (server.Properties["rcon.password"] is not { Length: > 0 })
            throw new MCExternalException("RCON password is not set for this minecraft server. To use RCON, enter a valid password.");

        return true;
    }
    
    
    private readonly CoreRCON.RCON _rcon;

    public bool IsConnected { get; private set; } = false;

    /// <summary>
    /// Creates a new RCON client
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    /// <param name="password"></param>
    public RconClient(string ip, ushort port, string password)
    {
        if (ip == "localhost")
            ip = "127.0.0.1";

        _rcon = new CoreRCON.RCON(IPAddress.Parse(ip), port, password);
    }

    /// <summary>
    /// Connects to the server
    /// </summary>
    /// <returns></returns>
    public async Task ConnectAsync()
    {
        _rcon.OnDisconnected += () => IsConnected = false;
        await _rcon.ConnectAsync();
        IsConnected = true;
    }

    /// <summary>
    /// Sends a single message and returns the response
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<string> SendMessageAsync(string message)
    {
        return await _rcon.SendCommandAsync(message);
    }


    /// <summary>
    /// Disposes the client
    /// </summary>
    public void Dispose()
    {
        _rcon.Dispose();
        IsConnected = false;
    } 
}
