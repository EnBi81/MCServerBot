using Loggers;
using Loggers.Loggers;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using System.Drawing;

namespace MCWebAPI.Utils.Images;

/// <summary>
/// Manages the icons for the server
/// </summary>
public class McIconManager
{
    private const string ErrorBarrierIcon = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAABySURBVDhPrY3bDcAgDAPD/tN0AZbpIC1WEyk0D4LESf6g9Tl0lN7acxOlQYfrM145yjXC2oe+HF4YhCM7shxzB/hp0LK8ywN/GZQHPBmUBiIZLAcyGaQDKxmEA1UZmQYg7MhmAOCDLmQxsoAflXD9BEQvGgfXopyOWU8AAAAASUVORK5CYII=";
   
    private const string IconFolder = "wwwroot\\icons";
    private const string IconFolderRelative = "icons";
    
    
    private readonly static object lockObject = new();
    
    private static List<McIcon>? _icons = null;
    
    private static async Task<List<McIcon>> GetIconsAsync()
    {
        if(_icons is null)
        {
            var icons = await LoadIconsAsync();
            lock (lockObject)
            {
                _icons ??= icons;
            }
        }

        return _icons;
    }

    private static async Task<List<McIcon>> LoadIconsAsync()
    {
        var icons = new List<McIcon>();

        if (!Directory.Exists(IconFolder))
            throw new MCInternalException("Could not find the icon folder");

        var files = Directory.GetFiles(IconFolder, "*", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var mcIcon = new McIcon(file);
            try
            {
                await mcIcon.InitializeAsync();
                icons.Add(mcIcon);
            }
            catch(Exception e)
            {
                LogService.GetService<WebApiLogger>().LogError("image", e);
                continue;
            }
        }

        return icons;
    }

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly WebApiLogger _logger = LogService.GetService<WebApiLogger>();

    public McIconManager(IHttpContextAccessor httpContext)
    {
        _httpContextAccessor = httpContext;
    }

    private async Task<List<string>> ConvertIconsToAbsoluteUri()
    {
        var icons = new List<string>();

        if (_httpContextAccessor.HttpContext == null)
            throw new MCInternalException("HttpContext is null");

        foreach (var icon in await GetIconsAsync())
        {
            var iconUrl = ConvertIconToAbsoluteUri(icon.FileName);
            icons.Add(iconUrl);
        } 

        return icons;
    }

    private string ConvertIconToAbsoluteUri(string icon)
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new MCInternalException("HttpContext is null");

        var uri = new UriBuilder(_httpContextAccessor.HttpContext.Request.Scheme,
            _httpContextAccessor.HttpContext.Request.Host.Host,
            _httpContextAccessor.HttpContext.Request.Host.Port ?? (_httpContextAccessor.HttpContext.Request.Scheme == "https" ? 443 : 80),
            Path.Combine(IconFolderRelative, icon));
        return uri.ToString();
    }


    /// <summary>
    /// Dont forget to close the stream.
    /// </summary>
    /// <param name="iconName"></param>
    /// <returns></returns>
    public async Task<(string extension, Stream stream)> GetIconOrErrorIconAsync(string? iconName)
    {
        List<McIcon> allIcons = await GetIconsAsync();

        McIcon? icon = allIcons.FirstOrDefault(i => i.FileName == iconName);

        try
        {

            if (icon is not null)
            {
                var path = Path.Combine(IconFolder, icon.FileName);
                var streamFile = File.OpenRead(path);
                return (icon.Extension, streamFile);
            }
        }
        catch (FileNotFoundException e) // icon is not on the disk
        {
            _logger.LogError("image", e);
        }

        // return the error icon
        var errorIcon = Convert.FromBase64String(ErrorBarrierIcon);
        var stream = new MemoryStream(errorIcon);
        return ("png", stream);
    }


    /// <summary>
    /// Gets all the icons converted into absolute url
    /// </summary>
    /// <returns></returns>
    public async Task<ICollection<string>> GetIcons()
    {
        ICollection<string> icons = await ConvertIconsToAbsoluteUri();
        return icons;
    }

    /// <summary>
    /// Deletes an icon
    /// </summary>
    /// <param name="icon">icon to delete</param>
    /// <returns></returns>
    /// <exception cref="MCExternalException"></exception>
    public async Task DeleteIcon(string? icon)
    {
        if (icon is null)
            throw new MCExternalException("Icon is null");


        var allIcons = await GetIconsAsync();
        
        var potencialIcons = allIcons.Where(i => i.FileName == icon);
        
        if (potencialIcons.Any())
        {
            var iconToDelete = potencialIcons.First();

            allIcons.Remove(iconToDelete);
            File.Delete(Path.Combine(IconFolder, iconToDelete.FileName));

            _logger.Log("image", "Deleted icon " + iconToDelete.ImagePath);
        }
        else
            throw new MCExternalException("Icon does not exist");
    }

    /// <summary>
    /// Creates a new icon
    /// </summary>
    /// <returns></returns>
    public async Task CreateIcon(IconUploadDto icon)
    {
        string? iconName = icon.IconName;

        // check if icon name is null
        if (iconName is null)
            throw new MCExternalException("Icon name is not null");

        // check if data is null
        if (icon.IconData is null)
            throw new MCExternalException("Icon data is null");

        
        var fileName = Path.Combine(IconFolder, iconName);

        _logger.Log("image", "Starting icon upload " + fileName);


        MemoryStream? memoryStream = null;

        // this catches if the base64 string is wrong, or if the image was in an incorrect format
        try
        {
            memoryStream = new MemoryStream(Convert.FromBase64String(icon.IconData));

#pragma warning disable CA1416
            using var img = Image.FromStream(memoryStream);

            // if image is not square
            if (img.Height != img.Width)
                throw new Exception($"Image height and width must be identical. Received: w{img.Width}xh{img.Height}");

            // if image is too big, then resize
            var maxSize = 500;


            if (img.Height > maxSize)
                throw new Exception($"Maximum image size is {maxSize}x{maxSize}. The received size is {img.Width}x{img.Height}");


            _logger.Log("image", "Saving icon " + fileName);
            img.Save(fileName);
#pragma warning restore CA1416
            

            McIcon mcIcon = new (fileName);
            try
            {
                await mcIcon.InitializeAsync();
                var allIcons = await GetIconsAsync();

                // add the correct extension for the file
                FileInfo info = new (fileName);

                if (!info.Extension.EndsWith(mcIcon.Extension))
                {
                    _logger.Log("image", $"Detected image extension '{mcIcon.Extension}' for " + fileName);

                    var correctName = info.Name[..^info.Extension.Length] + "." + mcIcon.Extension;
                    var correctFullName = Path.Join(info.DirectoryName, correctName);
                    info.MoveTo(correctFullName);

                    // set the correct file names
                    mcIcon.FileName = correctName;
                    mcIcon.ImagePath = correctFullName;
                }

                allIcons.Add(mcIcon);
            }
            catch
            {
                File.Delete(fileName);
                throw;
            }
            

            
        }
        catch (Exception e)
        {
            if (e.Message == "Parameter is not valid.")
                throw new MCExternalException("Invalid image format");
            throw new MCExternalException(e.Message);
        }
        finally
        {
            memoryStream?.Close();
        }
    }

}
