﻿using Application.Minecraft.MinecraftServers;
using Shared.Exceptions;
using Shared.Model;

namespace Application.Minecraft.States
{
    /// <summary>
    /// Blueprint of a Minecraft Server State.
    /// </summary>
    internal interface IServerState
    {
        /// <summary>
        /// Gets the status of the server.
        /// </summary>
        public ServerStatus Status { get; }
        /// <summary>
        /// Gets if the server process is running.
        /// </summary>
        public bool IsRunning { get; }
        /// <summary>
        /// Starts the server process if the server is offline.
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="MinecraftServerException">If the server is not Offline.</exception>
        public Task Start(string username);
        /// <summary>
        /// Stops the server if the server is online.
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="MinecraftServerException">If the server is not Online.</exception>
        public Task Stop(string username);
        /// <summary>
        /// Handles a log message, extracts data of it and calls events based on it.
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage);
        /// <summary>
        /// Writes a command to the minecraft server process if the server is online.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="username"></param>
        /// <exception cref="MinecraftServerException">If the server is not Online.</exception>
        public Task WriteCommand(string? command, string username);
    }
}
