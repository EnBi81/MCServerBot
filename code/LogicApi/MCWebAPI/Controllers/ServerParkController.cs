using APIModel;
using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Controllers.Utils;
using MCWebAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Model;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Controller for handling Server park related features.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ServerParkController : MCControllerBase
    {
        private readonly IServerPark serverPark;

        /// <summary>
        /// Initializes the controller
        /// </summary>
        /// <param name="serverPark">server park instance</param>
        public ServerParkController(IServerPark serverPark)
        {
            this.serverPark = serverPark;
        }


        /// <summary>
        /// Gets all the minecraft servers from the system.
        /// </summary>
        /// <returns>All the minecraft servers from the system.</returns>
        [HttpGet(Name = nameof(GetAllServers))]
        [ProducesResponseType(typeof(IEnumerable<MinecraftServerDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAllServers()
        {
            var servers = serverPark.MCServers.Values.Select(s => s.ToDTO());
            return Ok(servers);
        }

        /// <summary>
        /// Creates a server.
        /// </summary>
        /// <param name="data">data which are required to create the server</param>
        /// <returns></returns>
        [HttpPost(Name = nameof(CreateServer))]
        [ProducesResponseType(typeof(MinecraftServerDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateServer([FromBody] ServerCreationDto data)
        {
            var user = await GetUserEventData();

            IMinecraftServer server = await serverPark.CreateServer(data?.NewName, user);
            return Created("minecraftserver/" + server.Id, server.ToDTO());
        }

        /// <summary>
        /// Gets the currently running server
        /// </summary>
        /// <returns>the currently running server</returns>
        /// <response code="302">Redirects to the minecraft server api endpoint.</response>
        /// <response code="400">If there is no running server currently.</response>
        [HttpGet("running", Name = "GetRunningServer")]
        [ProducesResponseType(typeof(MinecraftServerDTO), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public IActionResult GetActiveServer()
        {
            if (serverPark.ActiveServer is not IMinecraftServer server || !server.IsRunning)
                throw new MCExternalException("There is no currently running server.");

            return RedirectToRoute("minecraftserver/" + server.Id);
        }
    }
}
