using Microsoft.AspNetCore.Mvc;

namespace MCWebAPI.Controllers.api.v1;

/// <summary>
/// Managing the icons
/// </summary>
[ApiVersion(ApiVersionV1)]
public class IconsController : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAllIcons()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> UploadIcon()
    {
        // https://stackoverflow.com/questions/10320232/how-to-accept-a-file-post
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteIcon()
    {
        throw new NotImplementedException();
    }
}
