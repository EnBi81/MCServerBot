using Application.MinecraftServer;
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
        [HttpGet]
        public IActionResult GetFullServer(string serverName)
        {
            if (!ServerPark.MCServers.TryGetValue(serverName, out var server))
                return GetBadRequest($"No server found with name '{serverName}'");

            return Ok(server);
        }

        [HttpDelete]
        public IActionResult DeleteServer(string serverName)
        {
            try
            {
                ServerPark.DeleteServer(serverName);
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
            if (!ServerPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (data == null)
                return GetBadRequest("No data has been provided");


            try
            {
                string newName = ControllerUtils.TryGetStringFromJson(data, "new-name");
                ServerPark.RenameServer(serverName, newName);

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
            if (!ServerPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (data == null)
                return GetBadRequest("No data has been provided");

            try
            {
                string command = ControllerUtils.TryGetStringFromJson(data, "command-data");
                ServerPark.MCServers[serverName].WriteCommand(command);
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
            if (!ServerPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            try
            {
                ServerPark.ToggleServer(serverName);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
