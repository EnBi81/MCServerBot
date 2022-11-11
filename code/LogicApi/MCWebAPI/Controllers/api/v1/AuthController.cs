using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Auth;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api.v1
{
    /// <summary>
    /// Authentication controller.
    /// </summary>
    [ApiVersion(ApiVersionV1)]
    public class AuthController : ApiController
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes the controller instance.
        /// </summary>
        /// <param name="authService">Auth Service</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        /// <summary>
        /// Generates a Bearer token and returns it.
        /// </summary>
        /// <param name="userLoginDto"></param>
        /// <returns></returns>
        /// <response code="200">Returns a <see cref="AuthenticatedResponse"/> object with the token included.</response>
        /// <response code="400">If the authentication fails.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticatedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto userLoginDto)
        {
            var authResponse = await _authService.Login(userLoginDto);

            return Ok(authResponse);
        }
    }
}
