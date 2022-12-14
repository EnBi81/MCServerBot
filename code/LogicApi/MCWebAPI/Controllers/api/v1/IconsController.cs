using APIModel.Responses;
using MCWebAPI.Utils.Images;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedPublic.DTOs;

namespace MCWebAPI.Controllers.api.v1;

/// <summary>
/// Managing the icons
/// </summary>
[ApiVersion(ApiVersionV1)]
public class IconsController : ApiController
{

    private readonly McIconManager _iconManager;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="iconManager"></param>
    public IconsController(McIconManager iconManager)
    {
        _iconManager = iconManager;
    }


    /// <summary>
    /// Gets all the available icons
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Returns a list of absolute urls to the server icons.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ICollection<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllIcons()
    {
        var icons = await _iconManager.GetIcons();
        return Ok(icons);
    }

    /// <summary>
    /// Uploads a new icon to the server
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [RequestSizeLimit(10_000_000)] // 10 MB
    public async Task<IActionResult> UploadIcons([FromBody] IconUploadDto icon)
    {
        await _iconManager.CreateIcon(icon);
        return Ok();
    }

    /// <summary>
    /// Deletes an existing icon.
    /// </summary>
    /// <returns></returns>
    [HttpDelete("{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteIcon([FromRoute] string name)
    {
        await _iconManager.DeleteIcon(name);

        return NoContent();
    }
}
