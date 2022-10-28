﻿using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.DTOs.Enums;
using System.Security.Claims;

namespace MCWebAPI.Controllers.Utils
{
    public class MCControllerBase : ControllerBase
    {
        public ulong ClaimUserId => ulong.Parse(User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Sid)).Value);
        public string ClaimUserName => User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Name)).Value;
        public Platform ClaimPlatform => Enum.Parse<Platform>(User.Claims.First(claim => claim.Type.Equals("Platform")).Value);


        public IActionResult GetBadRequest(string message)
        {
            var errorMessage = FormatErrorMessage(message);
            return BadRequest(errorMessage);
        }


        public static object FormatErrorMessage(string errorMessage)
        {
            return new { ErrorMessage = errorMessage };
        }

        public Task<UserEventData> GetUserEventData()
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
