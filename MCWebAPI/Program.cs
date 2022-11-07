using Loggers;
using Loggers.Loggers;
using MCWebAPI;
using MCWebAPI.APIExceptions;
using MCWebAPI.Utils;


LogService.CreateLogService()
                .AddLogger<NetworkLogger>()
                .AddLogger<HamachiLogger>()
                .AddLogger<MinecraftLogger>()
                .AddLogger<ConfigLogger>()
                .AddLogger<WebApiLogger>();

try
{
    MinecraftApi api = new();
    api.Run(args, Config.Instance);
}
catch(ConfigException e)
{
    LogService.GetService<ConfigLogger>().LogError(e);
}
catch(Exception e)
{
    LogService.GetService<WebApiLogger>().LogFatal(e);
}