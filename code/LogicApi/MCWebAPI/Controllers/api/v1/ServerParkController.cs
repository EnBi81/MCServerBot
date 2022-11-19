using APIModel.DTOs;
using APIModel.Responses;
using Application.Minecraft.Versions;
using MCWebAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Model;

namespace MCWebAPI.Controllers.api.v1
{
    /// <summary>
    /// Controller for handling Server park related features.
    /// </summary>
    [ApiVersion(ApiVersionV1)]
    public class ServerParkController : ApiController
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
        [ProducesResponseType(typeof(IEnumerable<IMinecraftServer>), StatusCodes.Status200OK)]
        public IActionResult GetAllServers()
        {
            var servers = serverPark.MCServers.Values;
            return Ok(servers);
        }

        /// <summary>
        /// Creates a server.
        /// </summary>
        /// <param name="data">data which are required to create the server</param>
        /// <returns></returns>
        [HttpPost(Name = nameof(CreateServer))]
        [ProducesResponseType(typeof(IMinecraftServer), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateServer([FromBody] ServerCreationDto data)
        {
            var user = await GetUserEventData();

            IMinecraftServer server = await serverPark.CreateServer(data, user);
            return Created("minecraftserver/" + server.Id, server);
        }

        /// <summary>
        /// Gets the currently running server
        /// </summary>
        /// <returns>the currently running server</returns>
        /// <response code="200">Returns the currently running server.</response>
        /// <response code="400">If there is no running server currently.</response>
        [HttpGet("running", Name = "GetRunningServer")]
        [ProducesResponseType(typeof(IMinecraftServer), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
        public IActionResult GetActiveServer()
        {
            if (serverPark.ActiveServer is not IMinecraftServer server || !server.IsRunning)
                throw new MCExternalException("There is no currently running server.");


            return Ok(server);
        }


        /// <summary>
        /// Gets all the available minecraft versions.
        /// </summary>
        /// <returns></returns>
        [HttpGet("versions")]
        [ProducesResponseType(typeof(IEnumerable<IMinecraftVersion>), StatusCodes.Status200OK)]
        public IActionResult GetAllVersions()
        {
            var versions = serverPark.MinecraftVersionCollection.GetAll();
            return Ok(versions);
        }

        /// <summary>
        /// Checks the internet if there are any new versions available, if yes, then adds them to the collection.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("versions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshVersions()
        {
            await serverPark.MinecraftVersionCollection.LoadAsync();
            return Ok();
        }
    }
}
