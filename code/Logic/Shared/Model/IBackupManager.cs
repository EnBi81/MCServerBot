
using SharedPublic.Enums;

namespace SharedPublic.Model
{
    public interface IBackupManager
    {
        /// <summary>
        /// Gets all the backups for a server
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public Task<IEnumerable<IBackup>> GetBackupsByServer(long serverId);
        /// <summary>
        /// Deletes a backup
        /// </summary>
        /// <param name="backup">backup to delete</param>
        /// <returns></returns>
        public Task DeleteBackup(IBackup backup);
        /// <summary>
        /// Creates and returns the backup filestream to which the backup should be saved
        /// </summary>
        /// <param name="serverId">id of the server to back up</param>
        /// <param name="name">name of the backup</param>
        /// <param name="isAutomatic">if the backup is automatic</param>
        /// <returns></returns>
        public Task<string> CreateBackupPath(long serverId, string name, BackupType type);
    }
}
