using SharedPublic.Model;

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
