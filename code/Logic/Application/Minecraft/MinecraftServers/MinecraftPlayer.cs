using Application.Minecraft.MinecraftServers.Utils;
using Loggers;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

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


    // RCON player attributes

    /// <inheritdoc/>
    public int? DeathTime { get; private set; }
    
    /// <inheritdoc/>
    public string? Dimension { get; private set; }

    /// <inheritdoc/>
    public byte? FoodLevel { get; private set; }

    /// <inheritdoc/>
    public byte? HealthLevel { get; private set; }

    /// <inheritdoc/>
    public byte? PlayerGameType { get; private set; }

    /// <inheritdoc/>
    public IPoint3D? Position { get; private set; }

    /// <inheritdoc/>
    public int? Score { get; private set; }

    /// <inheritdoc/>
    public bool? SeenCredits { get; private set; }

    /// <inheritdoc/>
    public byte? SleepTimer { get; private set; }

    /// <inheritdoc/>
    public int? XpLevel { get; private set; }





    private readonly Func<Task<RconClient>> _getRconClient;
    private DateTime _lastUpdated = DateTime.MinValue;
    
    public MinecraftPlayer(string username, Func<Task<RconClient>> client)
    {
        Username = username;
        OnlineFrom = null;
        PastOnlineTicks = 0;
        _getRconClient = client;

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
        // dont even try to update it while offline
        if (OnlineFrom is null)
            throw new MCExternalException("Player is not online");

        // dont update if the last update was half a minute ago.
        if ((DateTime.Now - _lastUpdated).TotalSeconds < 30)
            return;

        RconClient rconClient = await _getRconClient();

        string message = "data get entity " + Username;
        var result = await rconClient.SendMessageAsync(message);

        _lastUpdated = DateTime.Now;

        var playerData = new NBTPlayerData(result);

        DeathTime = playerData.DeathTime();
        Dimension = playerData.Dimension();
        FoodLevel = (byte?)playerData.FoodLevel();
        HealthLevel = (byte?)playerData.Health();
        PlayerGameType = (byte?)playerData.PlayerGameType();
        Score = playerData.Score();
        SleepTimer = (byte?)playerData.SleepTimer();
        XpLevel = playerData.XpLevel();

        var seenCredits = playerData.SeenCredits();
        SeenCredits = seenCredits switch
        {
            1 => true,
            0 => false,
            _ => null
        };

        
        if (playerData.Position() is { } pos)
        {
            Position = new Point3D
            {
                X = pos.X,
                Y = pos.Y,
                Z = pos.Z
            };
        }
        else Position = null;
    }


    private class Point3D : IPoint3D
    {
        public double X { get; init; }

        public double Y { get; init; }

        public double Z { get; init; }
    }

    private class NBTPlayerData
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
         *      DeathTime: 0s, 
         *      Dimension: \"minecraft:overworld\", 
         *      - EnderItems: [], 
         *      - FallDistance: 0.0f, 
         *      - FallFlying: 0b, 
         *      - Fire: -20s, 
         *      - foodExhaustionLevel: 0.15f, 
         *      foodLevel: 20, 
         *      - foodTickTimer: 0
         *      - foodSaturationLevel: 5.0f, 
         *      Health: 20.0f, 
         *      - HurtByTimestamp: 0, 
         *      - HurtTime: 0s, 
         *      - Inventory: [], 
         *      - Invulnerable: 0b, 
         *      - Motion: [0.0d, -0.0784000015258789d, 0.0d], 
         *      - OnGround: 1b, 
         *      playerGameType: 0, 
         *      Pos: [-58.81153211217702d, 89.0d, -124.52695435063991d], 
         *      - PortalCooldown: 0, 
         *      - recipeBook: {recipes: [], isBlastingFurnaceFilteringCraftable: 0b, isSmokerGuiOpen: 0b, isFilteringCraftable: 0b, toBeDisplayed: [], isFurnaceGuiOpen: 0b, isGuiOpen: 0b, isFurnaceFilteringCraftable: 0b, isBlastingFurnaceGuiOpen: 0b, isSmokerFilteringCraftable: 0b}, 
         *      - Rotation: [107.34737f, -9.298987f], 
         *      Score: 0, 
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

        private readonly string _data;
        
        public NBTPlayerData(string nbt)
        {
            _data = nbt;

            if (!IsMatch("has the following entity data:", out _))
                throw new MCExternalException("Player data is not valid");
        }

        private bool IsMatch(string pattern, out Match match) 
        {
            var regex = new Regex(pattern);
            match = regex.Match(_data);
            return match is { Success: true };
        }

        private int? GetIntValue(string key)
        {
            string regex = key + ": ([\\d]+)";

            if (!IsMatch(regex, out var match))
                return null;

            string value = match.Groups[1].Value;
            return int.Parse(value);
        }


        public string? Dimension()
        {
            string regex = "Dimension: [\\\\\\\"]{2}([^\\\\\\\"]+)[\\\\\\\"]{2}";

            if (!IsMatch(regex, out var match))
                return null;

            return match.Groups[1].Value;
        }

        public (int X, int Y, int Z)? Position()
        {
            var pattern = "Pos: [[](([-\\d]+)[.\\dd]*[,\\s]*)(([-\\d]+)[.\\dd]*[,\\s]*)(([-\\d]+)[.\\dd]*[,\\s]*)[\\]]";

            if (!IsMatch(pattern, out var match))
                return null;

            var x = int.Parse(match.Groups[2].Value);
            var y = int.Parse(match.Groups[4].Value);
            var z = int.Parse(match.Groups[6].Value);

            return (x, y, z);
        }

        public int? DeathTime() => GetIntValue("DeathTime");
        public int? FoodLevel() => GetIntValue("foodLevel");
        public int? Health() => GetIntValue("Health");
        public int? PlayerGameType() => GetIntValue("playerGameType");
        public int? Score() => GetIntValue("Score");
        public int? SeenCredits() => GetIntValue("seenCredits");
        public int? SleepTimer() => GetIntValue("SleepTimer");
        public int? XpLevel() => GetIntValue("XpLevel");
    }
}
