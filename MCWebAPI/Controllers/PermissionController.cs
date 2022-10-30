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


        [HttpPost("{id:ulong}/grant")]
        public async Task<IActionResult> GrantPermission([FromRoute] ulong id)
        {
            try
            {
                UserEventData userEventData = await GetUserEventData();
                await _permissionLogic.GrantPermission(id, userEventData);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPost("{id:ulong}/revoke")]
        public async Task<IActionResult> RevokePermission([FromRoute] ulong id)
        {
            try
            {
                UserEventData userEventData = await GetUserEventData();
                await _permissionLogic.RevokePermission(id, userEventData);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
