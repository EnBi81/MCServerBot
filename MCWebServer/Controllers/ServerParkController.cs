using Application.Minecraft;
using Application.Minecraft.MinecraftServers;
using Microsoft.AspNetCore.Mvc;

namespace MCWebServer.Controllers
{
    [Route("/v1")]
    public class ServerParkController : ControllerBase
    {
        [HttpGet("csa")]
        public string Get()
        {
            return "Lesson";
        }

        [HttpPost("/{name}")]
        public ActionResult<string> CreateServer(string name)
        {
            return "hi";

            try
            {
                IMinecraftServer server = ServerPark.CreateServer(name);
                var returnObject = new { server.ServerName };
                return Created("", returnObject);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
