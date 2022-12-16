using Loggers;
using SharedPublic.Model;

namespace Application.Minecraft.MinecraftServers;

/// <summary>
/// Holds information of a minecraft player in
/// </summary>
public class MinecraftPlayer : IMinecraftPlayer
{
    /// <summary>
    /// Username of the player
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// If the player is online, this holds the time joined to the server, else null.
    /// </summary>
    public DateTime? OnlineFrom { get; private set; }

    /// <summary>
    /// Sum of the time spent online from the previous sessions.
    /// </summary>
    public TimeSpan PastOnline { get; private set; }


    public MinecraftPlayer(string username)
    {
        Username = username;
        OnlineFrom = null;
        PastOnline = new TimeSpan(0, 0, 0);

        LogService.GetService<MinecraftLogger>().Log("player", $"Player created: " + username);
    }

    /// <summary>
    /// Set the player to online
    /// </summary>
    public void SetOnline()
    {
        OnlineFrom = DateTime.Now;
        LogService.GetService<MinecraftLogger>().Log("player", $"Player {Username} online");
    }


    /// <summary>
    /// Set the player to offline
    /// </summary>
    public void SetOffline()
    {
        if (OnlineFrom == null)
            return;

        PastOnline += DateTime.Now - OnlineFrom.Value;
        OnlineFrom = null;

        LogService.GetService<MinecraftLogger>().Log("player", $"Player {Username} offline");
    }
}
