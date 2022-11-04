using APIModel.APIExceptions;
using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Auth;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Enums;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Authentication controller.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AuthController : MCControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes the controller instance.
        /// </summary>
        /// <param name="config">Configuration of the web api</param>
        /// <param name="authService">Auth Service</param>
        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
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
        [ProducesResponseType(typeof(AuthenticatedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto userLoginDto)
        {
            foreach(var prop in userLoginDto.GetType().GetProperties())
            {
                if(prop.GetValue(userLoginDto) is null)
                    throw new LoginException("You must provide a " + prop.Name);
            }

            if (Enum.TryParse(userLoginDto.Platform, true, out Platform platform))
                throw new LoginException("Unrecognized platform: " + userLoginDto.Platform);

            DataUser user = await _authService.GetUser(userLoginDto.Token);
            string token = AuthUtils.GenerateJwt(user, platform, _config);

            var authResponse = new AuthenticatedResponse { Token = token };

            return Ok(authResponse);
        }
    }
}
