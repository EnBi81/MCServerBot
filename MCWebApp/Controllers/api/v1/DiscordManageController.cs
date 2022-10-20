using Application.PermissionControll;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers.api.v1
{
    [ApiController]
    [Route("api/v1/discord/user")]
    public class DiscordManageController : MCControllerBase
    {
        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshUserInfoAsync()
        {
            string? cookie = UserToken;

            if (cookie == null)
                return GetBadRequest("No access token to refresh");

            try
            {
                DiscordUser? user = await WebsitePermission.RefreshUser(cookie);

                if (user == null)
                    return GetBadRequest("No user found with this cookie");

                return Ok(user);
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
