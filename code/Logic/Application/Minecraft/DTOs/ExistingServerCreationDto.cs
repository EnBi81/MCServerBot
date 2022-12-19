using Application.DAOs.Database;
using Application.Minecraft.Configs;
using Loggers;

namespace Application.Minecraft.DTOs;

internal class ExistingServerCreationDto
{
    public required IMinecraftDataAccess DataAccess { get; init; }
    public required MinecraftLogger Logger { get; init; }
    public required string ServerFolderName { get; init; }
    public required MinecraftConfig Config { get; init; }
}
