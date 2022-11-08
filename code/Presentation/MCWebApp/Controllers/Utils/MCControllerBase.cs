using Application.DAOs.Database;
using DataStorage.DataObjects;
using DataStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers.Utils
{
    public class MCControllerBase : ControllerBase
    {
        protected IWebsiteEventRegister WebsiteEventRegister { get; set; }

        public string? UserToken
        {
            get
            {
                return Request.Cookies[WebConstants.AUTH_COOKIE_NAME];
            }
            set
            {
                if(value == null)
                    throw new ArgumentNullException(nameof(value));

                Response.Cookies.Append(WebConstants.AUTH_COOKIE_NAME, value);
            }
        }

        public async Task<UserEventData> GetUserEventData()
        {
            if (UserToken == null)
                throw new Exception("User is not logged in!");

            var user = await WebsiteEventRegister.GetUser(UserToken);
            if (user == null)
                throw new Exception($"Cannot find user {UserToken} in database");

            return new UserEventData { Id = user.Id, Username = user.Username, Platform = DataStorage.DataObjects.Enums.Platform.Website };
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
