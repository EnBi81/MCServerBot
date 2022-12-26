using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Model
{
    /// <summary>
    /// Simple Player object
    /// </summary>
    public interface IPlayerSimple
    {
        /// <summary>
        /// Username of the player
        /// </summary>
        string Username { get; }

        /// <summary>
        /// If the player is online, this holds the time joined to the server, else null.
        /// </summary>
        public DateTime? OnlineFrom { get; }

        /// <summary>
        /// Sum of the time spent online from the previous sessions.
        /// </summary>
        public long PastOnlineTicks { get; }
    }
}
