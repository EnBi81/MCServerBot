using Application.Minecraft.MinecraftServers;
using Application.Minecraft.States.Abstract;
using Application.Minecraft.States.Attributes;
using SharedPublic.DTOs;
using SharedPublic.Exceptions;
using SharedPublic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Minecraft.States
{
    [ManualState]
    internal class ServerCreationState : MaintenanceStateAbs
    {
        public ServerCreationState(MinecraftServerLogic server, object[] args) : base(server, args) { }

        

        public override async Task Apply()
        {
            if (args.Length == 0)
                throw new MCInternalException("No argument given for server creation state");

            if (args[0] is not MinecraftServerCreationPropertiesDto)
                args[0] = new MinecraftServerCreationPropertiesDto();

            var dto = (MinecraftServerCreationPropertiesDto)args[0];

            dto.ValidateAndRetrieveData();
            await CreateServerFiles();
            await _server.Properties.UpdateProperties(dto);

            await SetNewStateAsync<OfflineState>();
            _server.McServerInfos.Save(_server);
        }
    }
}
