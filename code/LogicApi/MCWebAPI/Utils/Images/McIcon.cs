using FileTypeChecker;
using FileTypeChecker.Abstracts;
using SharedPublic.Exceptions;

namespace MCWebAPI.Utils.Images;


/// <summary>
/// Represents a single image.
/// </summary>
public class McIcon
{
    private readonly static string[] allowedExtensions = new string[] { "png", "jpg", "jpeg", "gif", "bmp" };

    public string ImagePath { get; internal set; }
    public string Extension { get; private set; }
    public string FileName { get; internal set; }

    /// <summary>
    /// Initializes a file from an image
    /// </summary>
    /// <param name="path"></param>
    public McIcon(string path)
    {
        ImagePath = path;
        FileInfo info = new FileInfo(path);
        Extension = null!;
        FileName = info.Name;
    }

    /// <summary>
    /// Initializes a file from an image
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception">If the file is not supported</exception>
    public async Task InitializeAsync()
    {
        await using var fileStream = File.OpenRead(ImagePath);
        var isRecognizable = FileTypeValidator.IsTypeRecognizable(fileStream);

        if (!isRecognizable)
            throw new Exception($"The file {ImagePath} is not a valid image");
        
        var fileType = FileTypeValidator.GetFileType(fileStream);

        string fileExtension = fileType.Extension;

        if (!allowedExtensions.Contains(fileExtension))
            throw new Exception($"The file {ImagePath} has extension {fileExtension}, which is not allowed");


        Extension = fileExtension;
    }
}
