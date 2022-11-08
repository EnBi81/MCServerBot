﻿using APIModel.DTOs;
using APIModel.Responses;
using Application.Permissions;
using MCWebAPI.APIExceptions;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Controller for managing permissions.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class PermissionController : MCControllerBase
    {
        private readonly IPermissionLogic _permissionLogic;

        /// <summary>
        /// Initializing the PermissionController.
        /// </summary>
        /// <param name="permissionLogic">permission logic</param>
        public PermissionController(IPermissionLogic permissionLogic)
        {
            _permissionLogic = permissionLogic;
        }

        private static ulong ConvertStringToId(string id)
        {
            if (!ulong.TryParse(id, out ulong userId))
                throw new WebApiArgumentException("id must be a number.");

            return userId;
        }


        /// <summary>
        /// Grants permission to a user. Note: the user must have been registered before.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If the request got successfully handled.</response>
        /// <response code="400">An exception has been occured during the process.</response>
        [HttpPost("{id:regex(\\d{{18}})}/grant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GrantPermission([FromRoute] string id)
        {
             var userId = ConvertStringToId(id);
             
             UserEventData userEventData = await GetUserEventData();
             await _permissionLogic.GrantPermission(userId, userEventData);
             return Ok();
        }

        /// <summary>
        /// Revokes permission from a user. Note: the user must have been registered before.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">If the request got successfully handled.</response>
        /// <response code="400">An exception has been occured during the process.</response>
        [HttpPost("{id:regex(\\d{{18}})}/revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RevokePermission([FromRoute] string id)
        {
            var userId = ConvertStringToId(id);
            
            UserEventData userEventData = await GetUserEventData();
            await _permissionLogic.RevokePermission(userId, userEventData);
            return Ok();
        }
    }
}
