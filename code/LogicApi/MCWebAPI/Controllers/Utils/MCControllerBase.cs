using APIModel.DTOs;
using APIModel.Responses;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Enums;
using System.Security.Claims;

namespace MCWebAPI.Controllers.Utils
{
    public class MCControllerBase : ControllerBase
    {
        protected ulong ClaimUserId => ulong.Parse(User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Sid)).Value);
        protected string ClaimUserName => User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Name)).Value;
        protected Platform ClaimPlatform => Enum.Parse<Platform>(User.Claims.First(claim => claim.Type.Equals("Platform")).Value);


        protected IActionResult GetBadRequest(string message)
        {
            var errorMessage = new ExceptionDTO()
            {
                Message = message
            };

            return BadRequest(errorMessage);
        }



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
