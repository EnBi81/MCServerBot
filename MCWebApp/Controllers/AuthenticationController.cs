using Application.PermissionControll;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [Route("/login")]
        [HttpGet]
        public IActionResult Login([FromQuery] string? token, [FromQuery] string? redirectUrl)
        {
            if (token == null)
                return BadRequest("No login token provided");

            if (!WebsitePermission.HasAccess(token))
                return Unauthorized("This token is invalid.");

            Response.Cookies.Append(WebsitePermission.CookieName, token);

            try
            {
                if (redirectUrl != null)
                    return Redirect(redirectUrl);
            }
            catch { }

            return Redirect("/index");
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout([FromQuery] string? redirectUrl)
        {
            Response.Cookies.Delete(WebsitePermission.CookieName);

            try
            {
                if (redirectUrl != null)
                    return Redirect(redirectUrl);
            }
            catch { }

            return Redirect("/index");
        }
    }
}
