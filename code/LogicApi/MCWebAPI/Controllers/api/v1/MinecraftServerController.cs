using APIModel.DTOs;
using APIModel.Responses;
using MCWebAPI.Utils.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using System.IO;

namespace MCWebAPI.Controllers.api.v1;

/// <summary>
/// Endpoint for managing the minecraft servers.
/// </summary>
[ApiVersion(ApiVersionV1)]
public class MinecraftServerController : ApiController
{
    private const string RouteId = "{id:long}";



    private readonly IServerPark serverPark;
    private readonly McIconManager _iconManager;

    /// <summary>
    /// Initializes the minecraft server controller.
    /// </summary>
    /// <param name="serverPark"></param>
    public MinecraftServerController(IServerPark serverPark, McIconManager iconManager)
    {
        this.serverPark = serverPark;
        _iconManager = iconManager;
    }

    
    /// <summary>
    /// Throws external exception if the server does not exist.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private Task ThrowIfServerNotExists(long id)
    {
        _ = serverPark.GetServer(id);
        return Task.CompletedTask;
    }



    /// <summary>
    /// Gets the informations of a server.
    /// </summary>
    /// <param name="id">id of the server.</param>
    /// <returns></returns>
    /// <response code="200">Returns the requested server object.</response>
    /// <response code="400">The server with the specified id does not exist.</response>
    [HttpGet(RouteId, Name = "GetServer")]
    [ProducesResponseType(typeof(IMinecraftServer), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public IActionResult GetFullServer([FromRoute] long id)
    {
        var server = serverPark.GetServer(id);
        return Ok(server);
    }


    /// <summary>
    /// Deletes a server from the system.
    /// </summary>
    /// <param name="id">id of the server</param>
    /// <returns></returns>
    /// <response code="204">The server is deleted. Nothing more.</response>
    /// <response code="400">The server with the specified id does not exist or an exception happened during the deletion.</response>
    [HttpDelete(RouteId, Name = "DeleteServer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteServer([FromRoute] long id)
    {
        var user = await GetUserEventData();
        await serverPark.DeleteServer(id, user);
        return NoContent();
    }

    /// <summary>
    /// Modifies the server information.
    /// </summary>
    /// <param name="id">id of the server to modify</param>
    /// <param name="dto">new values</param>
    /// <returns></returns>
    /// <response code="204">The server is deleted. Nothing more.</response>
    /// <response code="400">The server with the specified id does not exist or an exception happened during the deletion.</response>
    [HttpPut(RouteId, Name = "ModifyServer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ModifyServer([FromRoute] long id, [FromBody] ModifyServerDto dto)
    {
        var user = await GetUserEventData();
        await serverPark.ModifyServer(id, dto, user);

        return NoContent();
    }



    /// <summary>
    /// Writes a command to the server.
    /// </summary>
    /// <param name="id">id of the minecraft server</param>
    /// <param name="commandDto">command data</param>
    /// <returns></returns>
    /// <response code="204">The command is executed.</response>
    /// <response code="400">The server with the specified id does not exist or an exception happened during the command execution.</response>
    [HttpPost(RouteId + "/commands", Name = "WriteCommandToServer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> WriteCommand([FromRoute] long id, [FromBody] CommandDto commandDto)
    {
        var server = serverPark.GetServer(id);

        string? command = commandDto?.Command;
        var response = await server.WriteCommand(command);
        return Ok(response);
    }


    /// <summary>
    /// Toggles the minecraft server on and off.
    /// </summary>
    /// <param name="id">id of the minecraft server.</param>
    /// <returns></returns>
    /// <response code="204">The server is either started or deleted, depending on the state of it.</response>
    /// <response code="400">The server with the specified id does not exist or an exception happened during the toggle.</response>
    [HttpPost(RouteId + "/toggle", Name = "ToggleServer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleServer([FromRoute] long id)
    {
        var user = await GetUserEventData();
        await serverPark.ToggleServer(id, user);
        return NoContent();
    }


    /// <summary>
    /// Backs up the server.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost(RouteId + "/backups", Name = "BackupServer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BackupServer([FromRoute] long id, [FromBody] BackupDto dto)
    {
        var server = serverPark.GetServer(id);
        var user = await GetUserEventData();

        await server.Backup(dto, user);

        return NoContent();
    }

    /// <summary>
    /// Gets all the backups of a server.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet(RouteId + "/backups", Name = "GetBackups")]
    [ProducesResponseType(typeof(IEnumerable<IBackup>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBackups([FromRoute] long id)
    {
        await ThrowIfServerNotExists(id);

        IEnumerable<IBackup> backups = await serverPark.BackupManager.GetBackupsByServer(id);
        return Ok(backups);
    }

    /// <summary>
    /// Deletes a backup of a server.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="backupName"></param>
    /// <returns></returns>
    [HttpDelete(RouteId + "/backups/{backupName:regex(^[[\\w\\W]])}", Name = "DeleteBackup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteBackup([FromRoute] long id, [FromRoute] string backupName)
    {
        await ThrowIfServerNotExists(id);

        var backup = (await serverPark.BackupManager.GetBackupsByServer(id)).FirstOrDefault(b => b.Name == backupName);

        if (backup == null)
            throw new MCExternalException("Backup does not exist.");

        await serverPark.BackupManager.DeleteBackup(backup);
        return Ok(backup);
    }

    /// <summary>
    /// Restores a backup of a server.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="backupName"></param>
    /// <returns></returns>
    [HttpPatch(RouteId + "/backups/{backupName:regex(^[[\\w\\W]])}", Name = "RestoreBackup")]
    public async Task<IActionResult> RestoreBackup([FromRoute] long id, [FromRoute] string backupName)
    {
        var server = serverPark.GetServer(id);
        var backups = await serverPark.BackupManager.GetBackupsByServer(id);

        var backup = backups.FirstOrDefault(b => b.Name == backupName);

        if (backup is null)
            throw new MCExternalException($"{backupName} does not exist for server {id}");

        await server.Restore(backup);

        return Ok();
    }

    /// <summary>
    /// Gets all the properties of a server
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet(RouteId + "/properties", Name = "GetProperties")]
    public async Task<IActionResult> GetProperties([FromRoute] long id)
    {
        var server = serverPark.GetServer(id);
        var properties = server.Properties;

        return Ok(properties);
    }

    /// <summary>
    /// Modifies the server properties
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPatch(RouteId + "/properties", Name = "ModifyProperties")]
    public async Task<IActionResult> ModifyProperties([FromRoute] long id, [FromBody] MinecraftServerPropertiesDto dto)
    {
        var server = serverPark.GetServer(id);
        await server.Properties.UpdatePropertiesAsync(dto);

        return Ok();
    }

    /// <summary>
    /// Gets a player's data on the server.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet(RouteId + "/players/{username}", Name = "GetPlayer")]
    public async Task<IActionResult> GetPlayerData([FromRoute] long id, [FromRoute] string username)
    {
        var server = serverPark.GetServer(id);
        if(!server.PlayersFull.TryGetValue(username, out var player))
        {
            return NotFound();
        }

        await player.RefreshData();
        return Ok(player);
    }

    /// <summary>
    /// Gets the icon of the server.
    /// </summary>
    /// <returns></returns>
    [HttpGet(RouteId + "/icon", Name = "GetIcon")]
    public async Task<IActionResult> GetServerIcon([FromRoute] long id)
    {
        var server = serverPark.GetServer(id);

        var (extension, stream) = await _iconManager.GetIconOrErrorIconAsync(server.ServerIcon);


        return new FileStreamResult(stream, "image/" + extension);
    }
}
