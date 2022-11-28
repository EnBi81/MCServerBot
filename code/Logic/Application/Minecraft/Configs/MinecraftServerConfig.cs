using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Configs
{
    public class MinecraftServerConfig
    {
        /// <summary>
        /// Max ram a minecraft server can take
        /// </summary>
        public required int ServerMaxRamMB { get; init; }
        /// <summary>
        /// Ram the server has when starts
        /// </summary>
        public required int ServerInitRamMB { get; init; }
        /// <summary>
        /// Maximum amount of automatic backups. If the limit is reached, the oldest automatic backup will be deleted
        /// </summary>
        public required int MaxAutoBackup { get; init; }
        /// <summary>
        /// Maximum amount of manual backups. If the limit is reached, the oldest manual backup will be deleted
        /// </summary>
        public required int MaxManualBackup { get; init; }
        /// <summary>
        /// Minimum server uptime to perform automatic backup when the server has shut down.
        /// </summary>
        public required int AutoBackupAfterUptimeMinute { get; init; }
    }
}
