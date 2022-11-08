﻿using Application.Minecraft.MinecraftServers;
using Shared.Model;

namespace Application.Minecraft.States
{
    internal abstract class ServerStateAbs : IServerState
    {
        protected readonly MinecraftServerLogic _server;
        public ServerStateAbs(MinecraftServerLogic server)
        {
            _server = server;
        }

        public abstract ServerStatus Status { get; }
        public abstract bool IsRunning { get; }

        public abstract void HandleLog(LogMessage logMessage);
        public abstract void Start(string username);
        public abstract void Stop(string username);
        public abstract void WriteCommand(string? command, string username);
    }
}