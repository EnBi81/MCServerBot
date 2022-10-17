using Application.Minecraft;
using Application.Minecraft.Enums;
using Application.Minecraft.MinecraftServers;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace MCWebApp.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/serverpark")]
    public class ServerParkController : MCControllerBase
    {
        [HttpGet]
        public IActionResult GetAllServers()
        {
            static object GetSimplifiedServer(IMinecraftServer server)
            {
                return new
                {
                    Name = server.ServerName,
                    ServerStatus = server.Status switch
                    {
                        ServerStatus.Offline => "offline",
                        ServerStatus.Starting => "starting",
                        ServerStatus.Online => "online",
                        _ => "shutting-down"
                    },
                    server.OnlinePlayers,
                    Logs = server.Logs.TakeLast(50),
                    OnlineFrom = server.OnlineFrom?.ToString("yyyy-MM-dd HH:mm:ss"),
                    server.StorageSpace
                };
            }

            var servers = ServerPark.MCServers.Values.Select(GetSimplifiedServer);
            return Ok(servers);
        }

        [HttpPost]
        public IActionResult CreateServer([FromBody] Dictionary<string, object?>? data)
        {
            if (data == null)
                return GetBadRequest("No data provided in the body.");

            try
            {
                string name = ControllerUtils.TryGetStringFromJson(data, "new-name");
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
