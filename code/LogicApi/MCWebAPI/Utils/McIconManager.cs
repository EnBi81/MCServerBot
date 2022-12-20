using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using System.Drawing;
using System.Drawing.Imaging;

namespace MCWebAPI.Utils;

/// <summary>
/// Manages the icons for the server
/// </summary>
public class McIconManager
{

    private static List<string>? _icons = null;
    private readonly static object lockObject = new();
    private const string IconFolder = "wwwroot/icons";
    private const string IconFolderRelative = "icons";

    private static List<string> Icons => _icons ??= LoadIcons();

    private static List<string> LoadIcons()
    {
        var icons = new List<string>();

        if (!Directory.Exists(IconFolder))
            throw new MCInternalException("Could not find the icon folder");

        var files = Directory.GetFiles(IconFolder, "*.png", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            if (fileName != null)
                icons.Add(fileName);
        }

        return icons;
    }

    private readonly IHttpContextAccessor _httpContextAccessor;

    public McIconManager(IHttpContextAccessor httpContext)
    {
        _httpContextAccessor = httpContext;
    }

    private List<string> ConvertIconsToAbsoluteUri()
    {
        var icons = new List<string>();

        if (_httpContextAccessor.HttpContext == null)
            throw new MCInternalException("HttpContext is null");

        foreach (var icon in Icons)
        {
            var iconUrl = ConvertIconToAbsoluteUri(icon);
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


    public Task<ICollection<string>> GetIcons()
    {
        ICollection<string> icons = ConvertIconsToAbsoluteUri();
        return Task.FromResult(icons);
    }

    /// <summary>
    /// Deletes an icon
    /// </summary>
    /// <param name="icon">icon to delete</param>
    /// <returns></returns>
    /// <exception cref="MCExternalException"></exception>
    public Task DeleteIcon(string? icon)
    {
        if (icon is null)
            throw new MCExternalException("Icon is null");

        // TODO check default

        if (Icons.Remove(icon))
            File.Delete(Path.Combine(IconFolder, icon));
        else
            throw new MCExternalException("Icon does not exist");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates a new icon
    /// </summary>
    /// <returns></returns>
    public Task CreateIcon(IconUploadDto icon)
    {
        string? iconName = icon.IconName;

        // check if icon name is null
        if (iconName is null)
            throw new MCExternalException("Icon name is not null");

        // check if data is null
        if (icon.IconData is null)
            throw new MCExternalException("Icon data is null");


        // check if icon name ends with the png, if not, then add it
        if (!iconName.EndsWith(".png"))
        {
            var lastDot = iconName.LastIndexOf(".");

            if (lastDot > -1)
                iconName = iconName[..lastDot];


            iconName += ".png";
        }



        var fileName = Path.Combine(IconFolder, iconName);


        MemoryStream? memoryStream = null;

        // this catches if the base64 string is wrong, or if the image was in an incorrect format
        try
        {
            memoryStream = new MemoryStream(Convert.FromBase64String(icon.IconData));

#pragma warning disable CA1416
            var img = Bitmap.FromStream(memoryStream);

            // if image is not square
            if (img.Height != img.Width)
                throw new Exception("Image height and width must be identical");

            // if image is too big, then resize
            if (img.Height > 300)
            {
                var newSize = 300;
                Bitmap resizedImage = new Bitmap(newSize, newSize);

                // Draw the original image onto the resized image using high quality resampling
                using (Graphics g = Graphics.FromImage(resizedImage))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.DrawImage(img, 0, 0, newSize, newSize);
                }

                img = resizedImage;
            }



            img.Save(fileName, ImageFormat.Png);
#pragma warning restore CA1416

            Icons.Add(iconName);
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

        return Task.CompletedTask;
    }

}
