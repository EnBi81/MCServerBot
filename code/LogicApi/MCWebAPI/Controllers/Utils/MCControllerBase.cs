using APIModel.DTOs;
using APIModel.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using SharedPublic.DTOs;
using SharedPublic.DTOs.Enums;
using System.Security.Claims;

namespace MCWebAPI.Controllers.Utils
{
    /// <summary>
    /// Base controller for every controller.
    /// </summary>
    public class MCControllerBase : ControllerBase
    {
        /// <summary>
        /// Gets the user id from the claims (assumes the user is logged in)
        /// </summary>
        protected ulong ClaimUserId => ulong.Parse(User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Sid)).Value);
        /// <summary>
        /// Gets the username from the claims (assumes the user is logged in)
        /// </summary>
        protected string ClaimUserName => User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Name)).Value;
        /// <summary>
        /// Gets the platform from the claims (assumes the user is logged in)
        /// </summary>
        protected Platform ClaimPlatform => Enum.Parse<Platform>(User.Claims.First(claim => claim.Type.Equals("Platform")).Value);

        

        /// <summary>
        /// Gets the user event data of the logged in user.
        /// </summary>
        /// <returns></returns>
        protected Task<UserEventData> GetUserEventData()
        {
            var userEventData = new UserEventData
            {
                Id = ClaimUserId,
                Username = ClaimUserName,
                Platform = ClaimPlatform
            };
            return Task.FromResult(userEventData);
        }
    }
}
