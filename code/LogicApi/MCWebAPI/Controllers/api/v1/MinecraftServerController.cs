using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace MCWebAPI.Controllers.api.v1
{
    /// <summary>
    /// Endpoint for managing the minecraft servers.
    /// </summary>
    public class MinecraftServerController : ApiV1Controller
    {
        private const string RouteId = "{id:long}";



        private readonly IServerPark serverPark;

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
        /// <response code="400">The server with the specified id does not exist.</response>
        [HttpGet(RouteId, Name = "GetServer")]
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
        /// <response code="204">The server is deleted. Nothing more.</response>
        /// <response code="400">The server with the specified id does not exist or an exception happened during the deletion.</response>
        [HttpDelete(RouteId, Name = "DeleteServer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteServer([FromRoute] long id)
        {
            var user = await GetUserEventData();
            await serverPark.DeleteServer(id, user);
            return NoContent();
        }

        /// <summary>
        /// Modifies the server information.
        /// </summary>
        /// <param name="id">id of the server to modify</param>
        /// <param name="dto">new values</param>
        /// <returns></returns>
        /// <response code="204">The server is deleted. Nothing more.</response>
        /// <response code="400">The server with the specified id does not exist or an exception happened during the deletion.</response>
        [HttpPut(RouteId, Name = "ModifyServer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyServer([FromRoute] long id, [FromBody] ModifyServerDto dto)
        {
            var user = await GetUserEventData();
            await serverPark.ModifyServer(id, dto.ToModelDto(), user);

            return NoContent();
        }



        /// <summary>
        /// Writes a command to the server.
        /// </summary>
        /// <param name="id">id of the minecraft server</param>
        /// <param name="commandDto">command data</param>
        /// <returns></returns>
        /// <response code="204">The command is executed.</response>
        /// <response code="400">The server with the specified id does not exist or an exception happened during the command execution.</response>
        [HttpPost(RouteId + "/commands", Name = "WriteCommandToServer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public IActionResult WriteCommand([FromRoute] long id, [FromBody] CommandDto commandDto)
        {
            var server = serverPark.GetServer(id);

            string? command = commandDto?.Command;
            server.WriteCommand(command);
            return NoContent();
        }


        /// <summary>
        /// Toggles the minecraft server on and off.
        /// </summary>
        /// <param name="id">id of the minecraft server.</param>
        /// <returns></returns>
        /// <response code="204">The server is either started or deleted, depending on the state of it.</response>
        /// <response code="400">The server with the specified id does not exist or an exception happened during the toggle.</response>
        [HttpPost(RouteId + "/toggle", Name = "ToggleServer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleServer([FromRoute] long id)
        {
            var user = await GetUserEventData();
            await serverPark.ToggleServer(id, user);
            return NoContent();
        }
    }
}
