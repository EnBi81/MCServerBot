using APIModel;
using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Endpoint for managing the minecraft servers.
    /// </summary>
    [ApiController]
    [Route("minecraftserver/{id:long}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MinecraftServerController : MCControllerBase
    {
        private IServerPark serverPark;

        /// <summary>
        /// Initializes the minecraft server controller.
        /// </summary>
        /// <param name="serverPark"></param>
        public MinecraftServerController(IServerPark serverPark)
        {
            this.serverPark = serverPark;
        }


        /// <summary>
        /// Gets the informations of a server.
        /// </summary>
        /// <param name="id">id of the server.</param>
        /// <returns></returns>
        /// <response code="200">Returns the requested server object.</response>
        /// <response code="400">The server with the specified name does not exist.</response>
        [HttpGet]
        [ProducesResponseType(typeof(MinecraftServerDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public IActionResult GetFullServer([FromRoute] long id)
        {
            var server = serverPark.GetServer(id);
            var dto = server.ToDTO();
            return Ok(dto);
        }


        /// <summary>
        /// Deletes a server from the system.
        /// </summary>
        /// <param name="id">id of the server</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteServer([FromRoute] long id)
        {
            var user = await GetUserEventData();
            await serverPark.DeleteServer(id, user);
            return Ok();
        }

        /// <summary>
        /// Modifies the server information.
        /// </summary>
        /// <param name="id">id of the server to modify</param>
        /// <param name="dto">new values</param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyServer([FromRoute] long id, [FromBody] ModifyServerDto dto)
        {
            var user = await GetUserEventData();
            await serverPark.RenameServer(id, dto?.NewName, user);

            return Ok();
        }



        /// <summary>
        /// Writes a command to the server.
        /// </summary>
        /// <param name="id">id of the minecraft server</param>
        /// <param name="commandDto">command data</param>
        /// <returns></returns>
        [HttpPost("commands")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public IActionResult WriteCommand([FromRoute] long id, [FromBody] CommandDto commandDto)
        {
            var server = serverPark.GetServer(id);

            string? command = commandDto?.Command;
            server.WriteCommand(command);
            return Ok();
        }


        /// <summary>
        /// Toggles the minecraft server on and off.
        /// </summary>
        /// <param name="id">id of the minecraft server.</param>
        /// <returns></returns>
        [HttpPost("toggle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleServer([FromRoute] long id)
        {
            var user = await GetUserEventData();
            await serverPark.ToggleServer(id, user);
            return Ok();
        }
    }
}
