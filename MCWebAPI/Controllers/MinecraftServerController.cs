using APIModel;
using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Endpoint for managin the minecraft servers.
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
            try
            {
                var user = await GetUserEventData();
                await serverPark.DeleteServer(id, user);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyServer([FromRoute] long id, [FromBody] ModifyServerDto dto)
        {
            if (dto == null || dto.NewName is null)
                return GetBadRequest("No data has been provided");


            try
            {
                var user = await GetUserEventData();
                await serverPark.RenameServer(id, dto.NewName, user);

                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }



        [HttpPost("commands")]
        public IActionResult WriteCommand([FromRoute] long id, [FromBody] CommandDto commandDto)
        {
            

            if (commandDto == null || commandDto.Command is null)
                return GetBadRequest("No data has been provided");

            try
            {
                var server = serverPark.GetServer(id);

                string command = commandDto.Command;
                server.WriteCommand(command);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }


        /// <summary>
        /// Toggles the minecraft server on and off.
        /// </summary>
        /// <param name="serverName">name of the </param>
        /// <returns></returns>
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleServer([FromRoute] long id)
        {

            try
            {
                var user = await GetUserEventData();
                await serverPark.ToggleServer(id, user);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
