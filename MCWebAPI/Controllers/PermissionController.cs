using APIModel.APIExceptions;
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

        private ulong ConvertStringToId(string id)
        {
            if (!ulong.TryParse(id, out ulong userId))
                throw new WebApiArgumentException("id must be a number.");

            return userId;
        }


        [HttpPost("{id:regex(\\d{{18}})}/grant")]
        public async Task<IActionResult> GrantPermission([FromRoute] string id)
        {
            try
            {
                var userId = ConvertStringToId(id);

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
                var userId = ConvertStringToId(id);

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
