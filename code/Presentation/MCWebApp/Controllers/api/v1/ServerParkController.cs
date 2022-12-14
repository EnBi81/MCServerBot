using Application.Minecraft;
using Application.Minecraft.Enums;
using Application.Minecraft.MinecraftServers;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace MCWebApp.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/serverpark")]
    public class ServerParkController : MCControllerBase
    {
        private IServerPark serverPark;

        public ServerParkController(IServerPark serverPark)
        {
            this.serverPark = serverPark;
        }



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

            var servers = serverPark.MCServers.Values.Select(GetSimplifiedServer);
            return Ok(servers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateServer([FromBody] Dictionary<string, object?>? data)
        {
            if (data == null)
                return GetBadRequest("No data provided in the body.");

            try
            {
                string name = ControllerUtils.TryGetStringFromJson(data, "new-name");
                var user = await GetUserEventData();

                IMinecraftServer server = await serverPark.CreateServer(name, user);
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
            if (serverPark.ActiveServer is not IMinecraftServer server || !server.IsRunning)
                return Ok(new { Message = "No running server." });

            return Redirect("/api/v1/minecraftserver/" + server.ServerName);
        }
    }
}
