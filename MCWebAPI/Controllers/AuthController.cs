using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Auth;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Enums;

namespace MCWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : MCControllerBase
    {
        private readonly IConfiguration config;
        private readonly IAuthService authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            this.config = config;
            this.authService = authService;
        }

        

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto userLoginDto)
        {
            try
            {
                foreach(var prop in userLoginDto.GetType().GetProperties())
                {
                    if(prop.GetValue(userLoginDto) is null)
                        throw new Exception("You must provide a " + prop.Name);
                }

                if (Enum.TryParse(userLoginDto.Platform, true, out Platform platform))
                    throw new Exception("Unrecognized platform: " + userLoginDto.Platform);

                DataUser user = await authService.GetUser(userLoginDto.Token);
                string token = AuthUtils.GenerateJwt(user, platform, config);

                var authResponse = new AuthenticatedResponse { Token = token };

                return Ok(authResponse);
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
