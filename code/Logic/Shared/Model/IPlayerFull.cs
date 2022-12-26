using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Model
{
    /// <summary>
    /// All the information of a player
    /// </summary>
    public interface IPlayerFull : IPlayerSimple
    {
        /// <summary>
        /// Number of ticks the mob has been dead for. Controls death animations. 0 when alive.
        /// </summary>
        int? DeathTime { get; }

        /// <summary>
        /// The ID of the dimension the player is in. Used to store the players last known location along with Pos.
        /// </summary>
        string? Dimension { get; }

        /// <summary>
        /// The value of the hunger bar; 20 is full.
        /// </summary>
        byte? FoodLevel { get; }

        /// <summary>
        /// Amount of health the entity has.
        /// </summary>
        byte? HealthLevel { get; }
        
        /// <summary>
        /// The game mode of the player. 0 is Survival, 1 is Creative, 2 is Adventure and 3 is Spectator.
        /// </summary>
        byte? PlayerGameType { get; }

        /// <summary>
        /// Position of the player.
        /// </summary>
        IPoint3D? Position { get; }

        /// <summary>
        /// The Score displayed upon death.
        /// </summary>
        int? Score { get; }

        /// <summary>
        /// 1 or 0 (true/false) - true if the player has entered the exit portal in the End at least once.
        /// </summary>
        bool? SeenCredits { get; }

        /// <summary>
        /// The number of ticks the player had been in bed. 0 when the player is not sleeping.
        /// In bed, increases up to 100, then stops. Skips the night after all players in bed have reached 100. 
        /// When getting out of bed, instantly changes to 100 and then increases for another 9 ticks (up to 109) before returning to 0.
        /// </summary>
        byte? SleepTimer { get; }

        /// <summary>
        /// The level shown on the XP bar.
        /// </summary>
        int? XpLevel { get; }


        /// <summary>
        /// Refreshes the data of the player
        /// </summary>
        /// <returns></returns>
        Task RefreshData();
    }
}
