using APIModel.DTOs;
using APIModel.Responses;
using Application.Permissions;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers
{

    [Route("[controller]")]
    [ApiController]
    [Authorize("DiscordBot")]
    [Produces("application/json")]
    public class DiscordController : MCControllerBase
    {
        private readonly IPermissionLogic permissionLogic;

        public DiscordController(IPermissionLogic permissionLogic)
        {
            this.permissionLogic = permissionLogic;
        }

        /// <summary>
        /// Gets the user token for by a Discord Id.
        /// </summary>
        /// <param name="id">Discord Id of the user.</param>
        /// <returns>A <see cref="UserTokenResponse"/> object.</returns>
        /// <response code="200">Returns a <see cref="UserTokenResponse"/> object.</response>
        /// <response code="400">If the user does not exist.</response>
        [HttpGet("token/{id:ulong}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetWebAccessToken([FromRoute] ulong id)
        {
            try
            {
                string token = await permissionLogic.GetToken(id);
                var userTokenResponse = new UserTokenResponse { UserToken = token };
                return Ok(userTokenResponse);
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }


        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
