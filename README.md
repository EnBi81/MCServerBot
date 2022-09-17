# MCServerBot
## Short Description
This project was created to make my friends able to start and stop a minecraft server which I hosted on my own computer. It started as a simple Discord bot, but eventually it became a much larger project than I thought. Currently, it hosts both a Discord bot, and a small ASP.Net core webserver, which my friends can visit through Hamachi, and manage the minecraft server.

This project is WIP.

## Latest info
Now the program is able to have not one, but many minecraft servers, this is only implemented in the Discord Bot, the webserver is still under development.

## Features:
(Currently these features are only implemented in the Discord bot):

- Start a minecraft server from the available servers (only one active server at a time)
- Stop the active minecraft server
- Add a new server
- Rename an existing server
- Delete an existing server (by moving all it's files to the DeletedServer folder.
- See the number of active players on the active server

Other features implemented in the webserver before adding the option to manage multiple servers:

- See all the players who joined to the server and their online time
- See all the output of the minecraft server (including the commands of the players)
- Write a command directly to the minecraft server process from the webpage

## Setup
To set up and run the program, download the whole project, go to the MCWebServer folder, create a config.json file, copy the json template from below and fill it out with your preferences/token/folder paths. Then you can start the .StartServer.cmd file.

```json
{
	"HamachiLocation": "C:\\Program Files (x86)\\LogMeIn Hamachi\\x64",
	"DiscordBotToken": "",
	"MinecraftServerHandlerPath": "MCServerHandler.exe",
	"MinecraftServersBaseFolder": "A:\\Games\\MinecraftServers\\",
	"MinecraftServerMaxRamMB": 8196,
	"MinecraftServerInitRamMB": 8196,
	"MinecraftServerPerformaceReportDelayInSeconds": 1,
	"JavaLocation": "C:\\Program Files\\Java\\jdk-17.0.2\\bin\\java.exe",
	"WebServerPortHttps": 443,
	"WebServerPortHttp": 2081
}
```

## MCServerHandler
MCServerHandler is working as a proxy for the minecraft server, due to the MCWebServer project is huge, and might contain bugs, or it may crash. It could possibly happen that the program crashes while the minecraft server stays online, and the only possible option to shut it down, is to kill the process from the Task Manager. To avoid that situation, MCServerHandler takes of the minecraft server's shutdown, by creating a thread that checks for the server.running file every 10 seconds. If this file is deleted, it will shut down the server the nicest way, so no progress is lost.

## MCWebServer
The brain of the program. It would be the best to separate the parts of this to separate programs, but as this project is so small, and we use it rarely, there is no point to split it to different servers.

### Config namespace
Handles reading config file, and quick verifying its content.

### Discord
Implementation of the Discord Bot

### Hamachi
Api for executing commands in the console to start/stop/get info from Hamachi

### Loggers
The whole system logs the events happened. This namespace takes care of logging them the right way.

### MinecraftServer
Keeps track of the minecraft servers, their status, the minecraft players, and the server's process's memory and cpu usage. Check the ServerPark.cs and the MinecraftServer.cs files

### Webpage for the webserver (WIP)
WIP

### PermissionControll
Manages permissions for both the web clients and the discord users.

### WebSocketHandler (WIP)
Handles the websockets (WIP)
