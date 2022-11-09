using Loggers;
using Loggers.Loggers;
using MCWebAPI;


LogService.CreateLogService()
                .AddLogger<NetworkLogger>()
                .AddLogger<HamachiLogger>()
                .AddLogger<MinecraftLogger>()
                .AddLogger<WebApiLogger>();

try
{
    MinecraftApi api = new();
    api.Run(args);
}
catch(Exception e)
{
    LogService.GetService<WebApiLogger>().LogFatal(e);
}