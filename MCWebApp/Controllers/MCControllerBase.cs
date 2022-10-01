﻿using Application.MinecraftServer;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    public class MCControllerBase : ControllerBase
    {
        public IActionResult GetBadRequest(string message)
        {
            var errorMessage = FormatErrorMessage(message);
            return BadRequest(message);
        }


        public static object FormatErrorMessage(string errorMessage)
        {
            return new { ErrorMessage = errorMessage };
        }
    }
}
