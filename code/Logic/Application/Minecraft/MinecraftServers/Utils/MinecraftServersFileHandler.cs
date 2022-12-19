using Application.Minecraft.Backup;
using Application.Minecraft.Util;
using SharedPublic.Enums;
using SharedPublic.Exceptions;
using SharedPublic.Model;

namespace Application.Minecraft.MinecraftServers.Utils;

public enum ServerFolder
{
    ServerFolder, TempTrash, TempBackup
}

/// <summary>
/// Handles files for a minecraft server
/// </summary>
public class MinecraftServersFileHandler
{
    private readonly McServerFileStructure _fileStructure;

    private string ServerFiles => _fileStructure.ServerFiles;
    private string BackupTempFolder => _fileStructure.TempBackup;
    private string TrashFolder => _fileStructure.TempTrash;

    public MinecraftServersFileHandler(McServerFileStructure fileStructure)
    {
        _fileStructure = fileStructure;
    }

    /// <summary>
    /// Returns the folder corresponding the enum value.
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private string GetFolderPath(ServerFolder folder)
    {
        return folder switch
        {
            ServerFolder.ServerFolder => ServerFiles,
            ServerFolder.TempBackup => BackupTempFolder,
            ServerFolder.TempTrash => TrashFolder,
            _ => throw new NotImplementedException()
        };
    }

    /// <summary>
    /// Returns the DirectoryInfo corresponding the enum value.
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    private DirectoryInfo GetDirectoryInfo(ServerFolder folder)
    {
        string path = GetFolderPath(folder);
        return new DirectoryInfo(path);
    }

    /// <summary>
    /// Accepts eula for a minecraft server.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="MCInternalException"></exception>
    public async Task AcceptEula()
    {
        string eulaPath = Path.Combine(ServerFiles, "eula.txt");
        if (!File.Exists(eulaPath))
            throw new MCInternalException("Couldn't find eula.txt in folder " + ServerFiles);


        string eulaText = await File.ReadAllTextAsync(eulaPath);

        if (!eulaText.Contains("eula=false"))
            return;

        eulaText = eulaText.Replace("eula=false", "eula=true");
        await File.WriteAllTextAsync(eulaPath, eulaText);
    }
    
    
    /// <summary>
    /// Moves files from a directory to a directory. If items parameter is null, it moves every files and folders.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="items"></param>
    public void MoveItems(ServerFolder from, ServerFolder to, string[]? items = null)
    {
        if(from == to)
            throw new ArgumentException("From and to folders are the same");

        var fromPath = GetDirectoryInfo(from);
        var toPath = GetDirectoryInfo(to);

        IEnumerable<FileInfo> filesToMove = fromPath.GetFiles().Where(f => f.Exists);
        IEnumerable<DirectoryInfo> foldersToMove = fromPath.GetDirectories().Where(f => f.Exists);

        if(items is not null)
        {
            filesToMove = filesToMove.Where(f => items.Any(item => f.Name.StartsWith(item)));
            foldersToMove = foldersToMove.Where(f => items.Any(item => f.Name.StartsWith(item)));
        }

        foreach (FileInfo file in filesToMove)
        {
            string newFileName = Path.Combine(toPath.FullName, file.Name);
            if(File.Exists(newFileName))
                File.Delete(newFileName);
            file.MoveTo(newFileName);
        }
        foreach (DirectoryInfo dir in foldersToMove)
        {
            string newFolderName = Path.Combine(toPath.FullName, dir.Name);
            if(Directory.Exists(newFolderName))
                Directory.Delete(newFolderName, true);
            dir.MoveTo(newFolderName);
        }
    }

    /// <summary>
    /// Deletes every file from a folder
    /// </summary>
    /// <param name="folder"></param>
    public void EmptyFolder(ServerFolder folder)
    {
        DirectoryInfo di = GetDirectoryInfo(folder);

        foreach (FileInfo file in di.GetFiles())
            file.Delete();
        foreach (DirectoryInfo dir in di.GetDirectories())
            dir.Delete(true);
    }


    /// <summary>
    /// Zips the server folder and saves it into the centralized backup folder.
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public async Task Backup(long serverId, string name, BackupType type)
    {
        var backupManager = BackupManager.Instance;

       
        // creating new backup
        string fromDir = ServerFiles;
        string backupPath = await BackupManager.Instance.CreateBackupPath(serverId, name, type);

        Predicate<string> filter = s => s.StartsWith("world");
        await FileHelper.CreateZipFromDirectory(fromDir, backupPath, System.IO.Compression.CompressionLevel.SmallestSize, false, filter);
    }

    /// <summary>
    /// Restores a backup from the centralized backup folder to the server folder. 
    /// WARNING: THIS WILL OVERWRITE ALL THE FILES IN THE SERVER FOLDER!
    /// </summary>
    /// <param name="serverId"></param>
    /// <param name="backup"></param>
    /// <returns></returns>
    /// <exception cref="MCExternalException"></exception>
    public async Task RestoreBackup(long serverId, IBackup backup)
    {
        var backupManager = BackupManager.Instance;
        var backupPath = await backupManager.CreateBackupPath(serverId, backup.Name, backup.Type);

        if (backup == null)
            throw new MCExternalException("Backup not found!");

        if (backup.ServerId != serverId)
            throw new MCExternalException("Backup does not belong to this server!");
        
        // extracting backup
        await FileHelper.ExtractToDirectory(backupPath, ServerFiles);
    }
}
