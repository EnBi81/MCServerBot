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
    /// Controller for handling Server park related features.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MinecraftServerDTO>), StatusCodes.Status200OK)]
        public IActionResult GetAllServers()
        {
            var servers = serverPark.MCServers.Values.Select(s => s.ToDTO());
            return Ok(servers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServer([FromBody] ServerCreationDto data)
        {
            var user = await GetUserEventData();

            IMinecraftServer server = await serverPark.CreateServer(data?.NewName, user);
            return CreatedAtRoute("minecraftserver/" + server.Id, server);
        }

        [HttpGet("running")]
        public IActionResult GetActiveServer()
        {
            if (serverPark.ActiveServer is not IMinecraftServer server || !server.IsRunning)
                return Ok(new { Message = "No running server." });

            return Redirect("minecraftserver/" + server.Id);
        }
    }
}
