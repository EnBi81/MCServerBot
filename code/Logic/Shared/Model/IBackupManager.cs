using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedPublic.Model
{
    public interface IBackupManager
    {
        public Task<IEnumerable<IBackup>> GetBackupsByServer(long serverId);
        public Task<string> GetBackupPath(IBackup backup);
        public Task DeleteBackup(IBackup backup);
        public Task<FileStream> CreateBackup(long serverId, string name, bool isAutomatic);
    }
}
