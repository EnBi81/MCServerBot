# MCServerBot
## Short Description
This project was created to make my friends able to 
start and stop a minecraft server which I hosted on my own computer. 
It started as a simple Discord bot, but eventually it became a much 
larger project than I thought. 

This project is WIP.

## Latest info
The clients and the server part has been separated, the clients (website and discord bot) 
does not work, they might not even compile, but the Web API works perfectly, and it is well documented.

## Features:
WEB API features:

- Log in (only available through the discord bot)
- Start a minecraft server from the available servers (only one active server at a time)
- Stop the active minecraft server
- Add a new server
- Rename an existing server
- Delete an existing server (by moving all it's files to the DeletedServer folder.
- Get all the players who joined to the server and their online time
- Get all the output of the minecraft server (including the commands of the players)
- Write a command directly to the minecraft server process from the webpage
- Grant permission for a discord user to use the system
- Revoke permission for a discord user to not allow to use the system



## Setup
To set up and run the program, download the whole project, 
go to the MCWebAPI folder, create a directory called 'Resources', 
inside it create a new config.json file, 
copy the json template from below and fill it out with 
your preferences/folder paths. Then you can start the .StartServer.cmd file.

```json
{
  "HamachiLocation": "C:\\..\\x64",
  "MinecraftServerHandlerPath": "C:\\..\\MCServerHandler.exe",
  "MinecraftServersBaseFolder": "C:\\..\\MinecraftServers\\",
  "MinecraftServerMaxRamMB": 8196,
  "MinecraftServerInitRamMB": 8196,
  "MinecraftMaxDiskSpaceGB": 5,
  "JavaLocation": "C:\\..\\java.exe",
  "WebApiPortHttps": 5001,
  "WebApiPortHttp": 5000
}
```

# Technical information

## - Project structure

### - - Data Tier

- DataStorageSQLite: Implementation of the data dao interfaces used in the application logic
- DataStorageTest: Unit Testing for the dao interfaces. Now it is set up for the sqlite tests.

### - - Logic

- Shared: Model interfaces, EventHandlers, Exceptions, DTOs which are used in several projects across the system.
- Loggers: Collection of the loggers which are responsible for logging in the system
- HamachiCli: uses command line to extract data from hamachi console client, and execute other commands
- MCServerHandler: proxy for the minecraft server, due to the MCWebServer project is huge, and might contain bugs, or it may crash. It could possibly happen that the program crashes while the minecraft server stays online, and the only possible option to shut it down, is to kill the process from the Task Manager. To avoid that situation, MCServerHandler takes of the minecraft server's shutdown, by creating a thread that checks for the server.running file every 10 seconds. If this file is deleted, it will shut down the server the nicest way, so no progress is lost.
- Application: The main logic of the system. I have spent a lot of time to make this part well structured and to be easily maintainable. For example, check out the double proxy system for the ServerPark in the Application.Minecraft:
  - ServerPark: logging events, registering events to the database, disabling functionalities till the instance is initialized.
  - ServerParkInputValidation: All the checking happens here. Checks if the server which needs to be started exists, or if it is possible to delete a server, and so on.
  - ServerParkLogic: all the methods contain the pure functionalities without any checking or validations, because they are done in the above layer.

### - - LogicAPI

- APIModel: model interfaces and exceptions which are required for the api. Collection of DTOs, request and response types.
- MCWebAPI: ASP.NET Core Web API

### - - Presentation (WIP)

- Discord bot
- Webserver (blazor and razor)
- Arduino (this is still in the idea phase, therefore this will be the last thing which will be implemented)