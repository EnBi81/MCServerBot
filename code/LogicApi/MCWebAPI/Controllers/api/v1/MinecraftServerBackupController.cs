using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace MCWebAPI.Controllers.api.v1
{
    public partial class MinecraftServerController
    {
        /// <summary>
        /// Backs up the server.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost(RouteId + "/backups", Name = "BackupServer")]
        public async Task<IActionResult> BackupServer([FromRoute] long id)
        {
            return Ok();
        }

        /// <summary>
        /// Gets all the backups of a server.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet(RouteId + "/backups", Name = "GetBackups")]
        public async Task<IActionResult> GetBackups([FromRoute] long id)
        {
            return Ok();
        }

        /// <summary>
        /// Deletes a backup of a server.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="backupName"></param>
        /// <returns></returns>
        [HttpDelete(RouteId + "/backups/{backupId:string}", Name = "DeleteBackup")]
        public async Task<IActionResult> DeleteBackup([FromRoute] long id, [FromRoute] string backupName)
        {
            return Ok();
        }

        /// <summary>
        /// Restores a backup of a server.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="backupName"></param>
        /// <returns></returns>
        [HttpPatch(RouteId + "/backups/{backupId:string}", Name = "RestoreBackup")]
        public async Task<IActionResult> RestoreBackup([FromRoute] long id, [FromRoute] string backupName)
        {
            return Ok();
        }
    }
}
