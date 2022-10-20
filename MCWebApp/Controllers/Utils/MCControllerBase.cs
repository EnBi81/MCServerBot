using Application.Minecraft;
using Application.PermissionControll;
using DataStorage.DataObjects;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers.Utils
{
    public class MCControllerBase : ControllerBase
    {
        public string? UserToken
        {
            get
            {
                return Request.Cookies[WebsitePermission.CookieName];
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                Response.Cookies.Append(WebsitePermission.CookieName, value);
            }
        }

        public async Task<UserEventData> GetUser()
        {
            return null!;
        }

        public IActionResult GetBadRequest(string message)
        {
            var errorMessage = FormatErrorMessage(message);
            return BadRequest(errorMessage);
        }


        public static object FormatErrorMessage(string errorMessage)
        {
            return new { ErrorMessage = errorMessage };
        }
    }
}
