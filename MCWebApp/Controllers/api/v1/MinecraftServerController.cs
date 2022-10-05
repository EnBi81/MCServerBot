using Application.MinecraftServer;
using MCWebApp.Controllers.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

            if (data.TryGetValue("new-name", out object? temp) &&
                temp is not string)
            {
                return GetBadRequest("'new-name' value is expected to be a string.");
            }

            try
            {
                if (temp is string newName)
                    ServerPark.RenameServer(serverName, newName);

                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }

        public class Command
        {
            public string commandData { get; set; }
        }

        [HttpPost("commands")]
        public IActionResult WriteCommand(string serverName, [FromBody] Dictionary<string, object?>? data)
        {
            if (!ServerPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (data == null)
                return GetBadRequest("No data has been provided");

            JsonValueKind? valueKind = null;
            if (data.TryGetValue("command-data", out object? temp) && // check if the jsonobject has this key, and get the value
                temp is JsonElement json &&   // check if the value is jsonelement (obviously it is, here we more just convert it to JsonElement)
                (valueKind = json.ValueKind) == JsonValueKind.String) // set the valuekind parameter to the received valuekind, and check if it is a string
            {
                try
                {
                    json = new JsonElement();
                    string? command = json.Deserialize<string>();

                    if(!string.IsNullOrWhiteSpace(command))
                        ServerPark.MCServers[serverName].WriteCommand(command);

                    return Ok();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return GetBadRequest(e.Message);
                }
            }

            return GetBadRequest($"'command-data' value is expected to be a string, but was {valueKind}.");
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
