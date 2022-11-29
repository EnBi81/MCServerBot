using SharedPublic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.Backup
{
    internal class Backup : IBackup
    {
        /// <inheritdoc/>
        public required long ServerId { get; init; }
        /// <inheritdoc/>
        public required string Name { get; init; }
        /// <inheritdoc/>
        public required bool IsAutomatic { get; init; }
        /// <inheritdoc/>
        public required DateTime CreationTime { get; init; }
        /// <inheritdoc/>
        public required long Size { get; init; }
    }
}
