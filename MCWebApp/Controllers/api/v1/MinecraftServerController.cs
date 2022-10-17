using Application.Minecraft;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static MCWebApp.Controllers.api.v1.MinecraftServerController;

namespace MCWebApp.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/minecraftserver/{serverName}")]
    public class MinecraftServerController : MCControllerBase
    {
        private IServerPark serverPark;

        public MinecraftServerController(IServerPark serverPark)
        {
            this.serverPark = serverPark;
        }


        [HttpGet]
        public IActionResult GetFullServer(string serverName)
        {
            if (!serverPark.MCServers.TryGetValue(serverName, out var server))
                return GetBadRequest($"No server found with name '{serverName}'");

            return Ok(server);
        }

        [HttpDelete]
        public IActionResult DeleteServer(string serverName)
        {
            try
            {
                serverPark.DeleteServer(serverName);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPut]
        public IActionResult ModifyServer(string serverName, [FromBody] Dictionary<string, object?>? data)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (data == null)
                return GetBadRequest("No data has been provided");


            try
            {
                string newName = ControllerUtils.TryGetStringFromJson(data, "new-name");
                serverPark.RenameServer(serverName, newName);

                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }



        [HttpPost("commands")]
        public IActionResult WriteCommand(string serverName, [FromBody] Dictionary<string, object?>? data)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (data == null)
                return GetBadRequest("No data has been provided");

            try
            {
                string command = ControllerUtils.TryGetStringFromJson(data, "command-data");
                serverPark.MCServers[serverName].WriteCommand(command);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }


        [HttpPost("toggle")]
        public IActionResult ToggleServer(string serverName)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            try
            {
                serverPark.ToggleServer(serverName);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
