using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using Application.Minecraft.Versions;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using System;

namespace Application.Minecraft.States;

[ManualState]
internal class ModificationState : MaintenanceStateAbs
{
    public ModificationState(MinecraftServerLogic server, object[] args) : base(server, args) { }
    
    public override async Task Apply()
    {
        ModifyServerDto dto = (ModifyServerDto)args[0];
        var mcVersionCollection = MinecraftVersionCollection.Instance;
        
        if (dto.NewName != null)
            _server.ServerName = dto.NewName;

        if (dto.Icon != null)
            _server.ServerIcon = dto.Icon;

        if (dto.Version is not null)
        {
            if (mcVersionCollection[dto.Version] is not IMinecraftVersion newVersion)
            {
                await SetNewStateAsync<OfflineState>();
                throw new MCExternalException("Could not find version " + dto.Version);
            }
                
            await SetNewStateAsync<VersionUpgradeState>(newVersion);
        }
        else
        {
            await SetNewStateAsync<OfflineState>();
        }
    }

    public override bool IsAllowedNextState(IServerState state) => base.IsAllowedNextState(state) || state is VersionUpgradeState;
}
