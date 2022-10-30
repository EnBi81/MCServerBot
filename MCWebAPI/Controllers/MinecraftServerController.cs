using APIModel.DTOs;
using MCWebAPI.Controllers.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Model;
using System.Xml;

namespace MCWebAPI.Controllers
{
    /// <summary>
    /// Endpoint for managin the minecraft servers.
    /// </summary>
    [ApiController]
    [Route("minecraftserver/{serverName:string}")]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MinecraftServerController : MCControllerBase
    {
        private IServerPark serverPark;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverPark"></param>
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
        public async Task<IActionResult> ModifyServer(string serverName, [FromBody] ModifyServerDto dto)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (dto == null || dto.NewName is null)
                return GetBadRequest("No data has been provided");


            try
            {
                var user = await GetUserEventData();
                await serverPark.RenameServer(serverName, dto.NewName, user);

                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }



        [HttpPost("commands")]
        public IActionResult WriteCommand(string serverName, [FromBody] CommandDto commandDto)
        {
            if (!serverPark.MCServers.ContainsKey(serverName))
                return GetBadRequest($"No server found with name '{serverName}'");

            if (commandDto == null || commandDto.Command is null)
                return GetBadRequest("No data has been provided");

            try
            {
                string command = commandDto.Command;
                serverPark.MCServers[serverName].WriteCommand(command);
                return Ok();
            }
            catch (Exception e)
            {
                return GetBadRequest(e.Message);
            }
        }


        /// <summary>
        /// Toggles the minecraft server on and off.
        /// </summary>
        /// <param name="serverName">name of the </param>
        /// <returns></returns>
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
