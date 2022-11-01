using Application.Permissions;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace MCWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionController : MCControllerBase
    {
        private readonly IPermissionLogic _permissionLogic;

        public PermissionController(IPermissionLogic permissionLogic)
        {
            _permissionLogic = permissionLogic;
        }


        [HttpPost("{id:regex(\\d{{18}})}/grant")]
        public async Task<IActionResult> GrantPermission([FromRoute] string id)
        {
            try
            {
                if (!ulong.TryParse(id, out ulong userId))
                    throw new Exception("id must be a number.");

                UserEventData userEventData = await GetUserEventData();
                await _permissionLogic.GrantPermission(userId, userEventData);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPost("{id:regex(\\d{{18}})}/revoke")]
        public async Task<IActionResult> RevokePermission([FromRoute] string id)
        {
            try
            {
                if (!ulong.TryParse(id, out ulong userId))
                    throw new Exception("id must be a number.");

                UserEventData userEventData = await GetUserEventData();
                await _permissionLogic.RevokePermission(userId, userEventData);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
