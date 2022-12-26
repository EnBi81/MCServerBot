using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Minecraft.MinecraftServers.Utils;

/// <summary>
/// Class managing the properties file for a minecraft server.
/// </summary>
public partial class MinecraftServerProperties : IMinecraftServerProperties
{
    /* 
     * server.properties file -----------------
     * Creating
     * Creating the server with servercreationproperties dto 
     * 
     * First start
     * 1. Start server 
     * 2. Create server files 
     * 3. read server.properties 
     * 4. modify the existing ones to the dto 
     * 
     * Server upgrade + first new start
     * 1. Save current properties 
     * 2. Create the new files
     * 3. Read the newly created properties
     * 4. Create a new servercreationproperties
     * 5. Apply the default properties
     * 6. Apply the previously saved properties from the previous version 
     * 
     * Restoring
     * 1. Ignore server.properties and basically everything except the world folder
     * 
     * ---------------------------------------------
     */

    [GeneratedRegex("[^=]+=[^=]*")]
    private static partial Regex PropertyRegex();



    /// <summary>
    /// Reads the file specified in the argument and creates a new instance of the MinecraftServerProperties.
    /// </summary>
    /// <param name="file">Path to the properties file.</param>
    /// <returns>Information of the properties loaded into a MinecraftServerProperties instance.</returns>
    public static MinecraftServerProperties GetProperties(string file, IMinecraftServer server)
    {
        return new MinecraftServerProperties(file, server);
    }

    /// <summary>
    /// Save a MinecraftServerProperties instance to a file.
    /// </summary>
    /// <param name="file">path to save the file</param>
    /// <param name="props">instance to save</param>
    public static async Task SaveProperties(string file, MinecraftServerProperties props)
    {
        StringBuilder sb = new();
        foreach (var (key, value) in props._properties)
            sb.AppendLine(key + "=" + value.ToString());

        await File.WriteAllTextAsync(file, sb.ToString());
    }

    
    /// <inheritdoc/>
    public Dictionary<string, string> Properties 
    {
        get
        {
            LoadData();
            return _properties;
        }
    }
    
    private readonly FileInfo _file;
    private DateTime lastReadTime = DateTime.MinValue;


    private readonly Dictionary<string, string> _properties = new();
    private readonly IMinecraftServer _server;


    /// <summary>
    /// Initializes the instance by splitting the lines to key value pairs and puts them into the Properties
    /// </summary>
    /// <param name="lines"></param>
    public MinecraftServerProperties(string file, IMinecraftServer server)
    {
        _file = new FileInfo(file);
        _server = server;
    }

    
    private void LoadData()
    {
        _file.Refresh(); // refresh to get the latest data
        if (lastReadTime > _file.LastWriteTime) // check if the last read time is newer than the last write time (data is already loaded)
            return;

        lastReadTime = DateTime.Now;
        _properties.Clear();

        string[] lines;
        try
        {
            lines = File.ReadAllLines(_file.FullName);
        }
        catch (FileNotFoundException)
        {
            lines = Array.Empty<string>();
        }
        
        Regex regex = PropertyRegex();
        foreach (var line in lines)
        {
            if (!regex.IsMatch(line))
                continue;

            string[] parts = line.Split('=');
            string key = parts[0];
            string value = parts[1];

            _properties.Add(key, value);
        }
    }
    

    /// <inheritdoc/>
    public string? this[string key]
    {
        get 
        {
            LoadData();
            _properties.TryGetValue(key, out string? value);
            return value;
        }
    }

    /// <inheritdoc/>
    public Dictionary<string, string>.Enumerator GetEnumerator()
        => Properties.GetEnumerator();

    public Task UpdatePropertiesAsync(MinecraftServerPropertiesDto dto)
    {
        var newProps = dto.ValidateAndRetrieveData();
        return UpdatePropertiesAsync(newProps);
    }

    /// <summary>
    /// Updates the properties with the new values. Only allowed if the server is offline or in maintenance mode.
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    public async Task UpdatePropertiesAsync(Dictionary<string, string> props)
    {
        if (_server.StatusCode is not ServerStatus.Offline and not ServerStatus.Maintenance)
            throw new MCExternalException("Updating server properties is only allowed in offline mode.");

        LoadData();

        foreach (var (key, value) in props)
        {
            if (_properties.ContainsKey(key))
                _properties[key] = value;
        }

        await SaveProperties(_file.FullName, this);
    }

    public void ClearProperties()
    {
        _properties.Clear();
    }

    
}
