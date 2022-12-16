﻿using SharedPublic.DTOs;
using SharedPublic.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Minecraft.MinecraftServers.Utils;

/// <summary>
/// Class managing the properties file for a minecraft server.
/// </summary>
public class MinecraftServerProperties : IMinecraftServerProperties
{
    /// <summary>
    /// Reads the file specified in the argument and creates a new instance of the MinecraftServerProperties.
    /// </summary>
    /// <param name="file">Path to the properties file.</param>
    /// <returns>Information of the properties loaded into a MinecraftServerProperties instance.</returns>
    public static MinecraftServerProperties GetProperties(string file)
    {
        return new MinecraftServerProperties(file);
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


    private readonly static string[] _visibleProperties = new string[]
    {
        "allow-flight",
        "difficulty",
        "enable-command-block",
        "enforce-secure-profile",
        "gamemode",
        "hardcore",
        "level-seed",
        "level-type",
        "max-world-size",
        "motd",
        "pvp",
        "simulation-distance",
        "spawn-monsters",
        "spawn-protection",
        "view-distance",
        "white-list"
    };


    /// <inheritdoc/>
    public Dictionary<string, string> Properties 
    {
        get
        {
            LoadData();

            return _properties
            .Where(pair => _visibleProperties.Contains(pair.Key))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    private readonly string _file;


    private readonly Dictionary<string, string> _properties = new();


    /// <summary>
    /// Initializes the instance by splitting the lines to key value pairs and puts them into the Properties
    /// </summary>
    /// <param name="lines"></param>
    public MinecraftServerProperties(string file)
    {
        _file = file;
    }


    private void LoadData()
    {
        if (_properties.Any())
            return;
        
        string[] lines;
        try
        {
            lines = File.ReadAllLines(_file);
        }
        catch (FileNotFoundException)
        {
            lines = Array.Empty<string>();
        }
        
        Regex regex = new("[^=]+=[^=]*");
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

    public async Task UpdateProperties(MinecraftServerPropertiesDto dto)
    {
        LoadData();

        foreach (var (key, value) in dto.ValidateAndRetrieveData())
        {
            if (_properties.ContainsKey(key))
                _properties[key] = value;
        }

        await SaveProperties(_file, this);
    }
}
