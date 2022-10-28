using Application.Permissions;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedAuth.DTOs;

namespace MCWebAPI.Controllers.api.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize("DiscordBot")]
    public class DiscordController : MCControllerBase
    {
        private readonly IPermissionLogic permissionLogic;

        public DiscordController(IPermissionLogic permissionLogic)
        {
            this.permissionLogic = permissionLogic;
        }


        [HttpGet("token/{id:ulong}")]
        public async Task<IActionResult> GetWebAccessToken([FromRoute] ulong id)
        {
            try
            {
                string token = await permissionLogic.GetToken(id);
                return Ok(token);
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                await permissionLogic.RegisterUser(registerDto.Id, registerDto.DiscordName, registerDto.ProfilePic);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPut("user")]
        public async Task<IActionResult> RefreshUser([FromBody] RegisterDto registerDto)
        {
            try
            {
                await permissionLogic.RefreshUser(registerDto.Id, registerDto.DiscordName, registerDto.ProfilePic);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
