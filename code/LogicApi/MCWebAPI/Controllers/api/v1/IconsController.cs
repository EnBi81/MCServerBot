using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api.v1;

/// <summary>
/// Managing the icons
/// </summary>
[ApiVersion(ApiVersionV1)]
public class IconsController : ApiController
{
    /// <summary>
    /// Gets all the available icons
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpGet]
    public async Task<IActionResult> GetAllIcons()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uploads a new icon to the server
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpPost]
    public async Task<IActionResult> UploadIcon()
    {
        // https://stackoverflow.com/questions/10320232/how-to-accept-a-file-post
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes an existing icon.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    [HttpDelete]
    public async Task<IActionResult> DeleteIcon()
    {
        throw new NotImplementedException();
    }
}
