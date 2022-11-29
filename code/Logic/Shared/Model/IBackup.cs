using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Model
{
    /// <summary>
    /// Represents a backup of a minecraft server
    /// </summary>
    public interface IBackup
    {
        /// <summary>
        /// Id of the server this backup belongs to.
        /// </summary>
        public long ServerId { get; }
        /// <summary>
        /// Name of the backup.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets if the backup was an automatic or manual backup.
        /// </summary>
        public bool IsAutomatic { get; }
        /// <summary>
        /// Date of the backup.
        /// </summary>
        public DateTime CreationTime { get; }
        /// <summary>
        /// Gets the size of the backup in bytes.
        /// </summary>
        public long Size { get; }
    }
}
