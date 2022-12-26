using Application.Minecraft.MinecraftServers.Utils;
using Loggers;
using SharedPublic.Model;

namespace Application.Minecraft.MinecraftServers;

/// <summary>
/// Holds information of a minecraft player in
/// </summary>
internal class MinecraftPlayer : IPlayerFull
{
    /// <inheritdoc/>
    public string Username { get; }

    /// <inheritdoc/>
    public DateTime? OnlineFrom { get; private set; }

    /// <inheritdoc/>
    public long PastOnlineTicks { get; private set; }

    /// <inheritdoc/>
    public string? Dimension { get; }

    /// <inheritdoc/>
    public byte? FoodLevel { get; }

    /// <inheritdoc/>
    public byte? PlayerGameType { get; }

    /// <inheritdoc/>
    public bool? SeenCredits { get; }

    /// <inheritdoc/>
    public byte? SleepTimer { get; }

    /// <inheritdoc/>
    public int? XpLevel { get; }



    private readonly Func<Task<RconClient>> getRconClient;
    
    public MinecraftPlayer(string username, Func<Task<RconClient>> client)
    {
        Username = username;
        OnlineFrom = null;
        PastOnlineTicks = 0;
        getRconClient = client;

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

        PastOnlineTicks += (DateTime.Now - OnlineFrom.Value).Ticks;
        OnlineFrom = null;

        LogService.GetService<MinecraftLogger>().Log("player", $"Player {Username} offline");
    }

    public async Task RefreshData()
    {
        RconClient rconClient = await getRconClient();

        string message = "data get entity " + Username;
        var result = await rconClient.SendMessageAsync(message);
        
        
    }


    private class JsonPlayerData
    {
        /*
         * Enbi81 has the following entity data: 
         *  {
         *      - AbsorptionAmount: 0.0f, 
         *      - abilities: {invulnerable: 0b, mayfly: 0b, instabuild: 0b, walkSpeed: 0.1f, mayBuild: 1b, flying: 0b, flySpeed: 0.05f}, 
         *      - Air: 300s, 
         *      - Attributes: [{Base: 0.10000000149011612d, Name: \"minecraft:generic.movement_speed\"}], 
         *      - Brain: {memories: {}}, 
         *      - DataVersion: 3120, 
         *    . DeathTime: 0s, 
         *      Dimension: \"minecraft:overworld\", 
         *      - EnderItems: [], 
         *      - FallDistance: 0.0f, 
         *      - FallFlying: 0b, 
         *      - Fire: -20s, 
         *      - foodExhaustionLevel: 0.15f, 
         *      foodLevel: 20, 
         *      - foodTickTimer: 0
         *      - foodSaturationLevel: 5.0f, 
         *    . Health: 20.0f, 
         *      - HurtByTimestamp: 0, 
         *      - HurtTime: 0s, 
         *      - Inventory: [], 
         *      - Invulnerable: 0b, 
         *      - Motion: [0.0d, -0.0784000015258789d, 0.0d], 
         *      - OnGround: 1b, 
         *      playerGameType: 0, 
         *    . Pos: [-58.81153211217702d, 89.0d, -124.52695435063991d], 
         *      - PortalCooldown: 0, 
         *      - recipeBook: {recipes: [], isBlastingFurnaceFilteringCraftable: 0b, isSmokerGuiOpen: 0b, isFilteringCraftable: 0b, toBeDisplayed: [], isFurnaceGuiOpen: 0b, isGuiOpen: 0b, isFurnaceFilteringCraftable: 0b, isBlastingFurnaceGuiOpen: 0b, isSmokerFilteringCraftable: 0b}, 
         *      - Rotation: [107.34737f, -9.298987f], 
         *    . Score: 0, 
         *      seenCredits: 0b, 
         *      - SelectedItemSlot: 0, 
         *      SleepTimer: 0s, 
         *      - UUID: [I; 1053829686, -157990484, -1340950600, -1871514253], 
         *      - XpTotal: 0,
         *      XpLevel: 0, 
         *      - warden_spawn_tracker: {warning_level: 0, ticks_since_last_warning: 1282, cooldown_ticks: 0}, 
         *      - XpP: 0.0f, 
         *      - XpSeed: 0, 
         *  }
         * 
         */ 
    }
}
