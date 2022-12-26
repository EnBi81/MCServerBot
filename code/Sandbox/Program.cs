using System.Net;
using System.Text.RegularExpressions;

namespace Sandbox
{
    public class SandBoxClass
    {
        static async Task Main(string[] args)
        {
            string regex = "Dimension: [\\\\\\\"]{2}([^\\\\\\\"]+)[\\\\\\\"]{2}";

            if (!IsMatch(regex, out var match))
                return;

            Console.WriteLine(match.Groups[1].Value);
        }

        private static bool IsMatch(string pattern, out Match match)
        {
            var regex = new Regex(pattern);
            match = regex.Match(text);
            return match is { Success: true };
        }


        static string text = @"{
               - AbsorptionAmount: 0.0f, 
               - abilities: {invulnerable: 0b, mayfly: 0b, instabuild: 0b, walkSpeed: 0.1f, mayBuild: 1b, flying: 0b, flySpeed: 0.05f}, 
               - Air: 300s, 
               - Attributes: [{Base: 0.10000000149011612d, Name: \""minecraft:generic.movement_speed\""}], 
               - Brain: {memories: {}}, 
               - DataVersion: 3120, 
               DeathTime: 0s, 
               Dimension: \""minecraft:overworld\"", 
               - EnderItems: [], 
               - FallDistance: 0.0f, 
               - FallFlying: 0b, 
               - Fire: -20s, 
               - foodExhaustionLevel: 0.15f, 
               foodLevel: 20, 
               - foodTickTimer: 0
               - foodSaturationLevel: 5.0f, 
               Health: 20.0f, 
               - HurtByTimestamp: 0, 
               - HurtTime: 0s, 
               - Inventory: [], 
               - Invulnerable: 0b, 
               - Motion: [0.0d, -0.0784000015258789d, 0.0d], 
               - OnGround: 1b, 
               playerGameType: 0, 
               Pos: [-58.81153211217702d, 89.0d, -124.52695435063991d], 
               - PortalCooldown: 0, 
               - recipeBook: {recipes: [], isBlastingFurnaceFilteringCraftable: 0b, isSmokerGuiOpen: 0b, isFilteringCraftable: 0b, toBeDisplayed: [], isFurnaceGuiOpen: 0b, isGuiOpen: 0b, isFurnaceFilteringCraftable: 0b, isBlastingFurnaceGuiOpen: 0b, isSmokerFilteringCraftable: 0b}, 
               - Rotation: [107.34737f, -9.298987f], 
               Score: 0, 
               seenCredits: 0b, 
               - SelectedItemSlot: 0, 
               SleepTimer: 0s, 
               - UUID: [I; 1053829686, -157990484, -1340950600, -1871514253], 
               - XpTotal: 0,
               XpLevel: 0, 
               - warden_spawn_tracker: {warning_level: 0, ticks_since_last_warning: 1282, cooldown_ticks: 0}, 
               - XpP: 0.0f, 
               - XpSeed: 0, 
           }";
    }
}

