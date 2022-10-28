using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;

namespace MCWebAPI.Controllers.api.v1
{
    [ApiController]
    [Route("/api/v1/minecraftserver/{serverName}")]
    [Authorize]
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
        public async Task<IActionResult> DeleteServer(string serverName)
        {
            try
            {
                var user = await GetUserEventData();
                await serverPark.DeleteServer(serverName, user);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task <IActionResult> ModifyServer(string serverName, [FromBody] Dictionary<string, object?>? data)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (data == null)
                return GetBadRequest("No data has been provided");


            try
            {
                string newName = ControllerUtils.TryGetStringFromJson(data, "new-name");
                var user = await GetUserEventData();

                await serverPark.RenameServer(serverName, newName, user);

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
        public async Task<IActionResult> ToggleServer(string serverName)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            try
            {
                var user = await GetUserEventData();
                await serverPark.ToggleServer(serverName, user);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }
    }
}
