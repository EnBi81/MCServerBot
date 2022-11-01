using APIModel.DTOs;
using APIModel.Responses;
using Application.Permissions;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers
{

    /// <summary>
    /// Controller for handling requests from the Discord Bot.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize("DiscordBot")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class DiscordController : MCControllerBase
    {
        private readonly IPermissionLogic permissionLogic;

        /// <summary>
        /// Initializes the DiscordController.
        /// </summary>
        /// <param name="permissionLogic"></param>
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
        [ProducesResponseType(typeof(UserTokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
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


        /// <summary>
        /// Register a discord user to the system.
        /// </summary>
        /// <param name="registerDto">Registered user data.</param>
        /// <returns></returns>
        /// <response code="200">Returns Ok 200.</response>
        /// <response code="400">If the DiscordName or the ProfilePic is null.</response>
        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserDataDto registerDto)
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

        /// <summary>
        /// Refreshes a user's data.
        /// </summary>
        /// <param name="userDto">Data of the user.</param>
        /// <returns></returns>
        /// <response code="200">Returns Ok 200.</response>
        /// <response code="400">If the DiscordName or the ProfilePic is null, or the user is not registered.</response>
        [HttpPut("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshUser([FromBody] UserDataDto userDto)
        {
            try
            {
                await permissionLogic.RefreshUser(userDto.Id, userDto.DiscordName, userDto.ProfilePic);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
