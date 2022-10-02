using Application.MinecraftServer;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers
{
    [ApiController]
    [Route("/api/v1/serverpark")]
    public class ServerParkController : MCControllerBase
    {
        [HttpGet]
        public IActionResult GetAllServers()
        {
            List<IMinecraftServerSimplified> servers = new (ServerPark.MCServers.Values);
            return Ok(servers);
        }

        [HttpPost]
        public IActionResult CreateServer([FromBody] Dictionary<string, object?>? data)
        {
            if (data == null)
                return GetBadRequest("No data provided in the body.");

            if (!data.TryGetValue("new-name", out object? nameData))
                return GetBadRequest("No 'new-name' key specified.");

            if (nameData is not string name)
                return GetBadRequest("Invalid name specified");

            try
            {
                IMinecraftServer server = ServerPark.CreateServer(name);
                return CreatedAtRoute("/api/v1/minecraftserver/" + server.ServerName, server); 
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpGet("running")]
        public IActionResult GetActiveServer()
        {
            if (ServerPark.ActiveServer is not IMinecraftServer server || !server.IsRunning)
                return Ok(new { Message = "No running server." });

            return Redirect("/api/v1/minecraftserver/" + server.ServerName);
        }
    }
}
