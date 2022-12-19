using Application.DAOs.Database;
using Application.Minecraft.Configs;
using Application.Minecraft.Versions;
using Loggers;
using SharedPublic.DTOs;

namespace Application.Minecraft.DTOs;

internal class NewServerCreationDto
{
    public required IMinecraftDataAccess DataAccess { get; init; }
    public required MinecraftLogger Logger { get; init; }
    public required long Id { get; init; }
    public required string ServerName { get; init; }
    public required string ServerFolderName { get; init; }
    public required string? ServerIcon { get; init; }
    public required MinecraftConfig Config { get; init; }
    public required IMinecraftVersion Version { get; init; }
    public required MinecraftServerCreationPropertiesDto? CreationProperties { get; init; }
}
