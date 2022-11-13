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
    Console.ForegroundColor = ConsoleColor.Black;
    Console.BackgroundColor = ConsoleColor.Cyan;

    Console.WriteLine("Starting application...");
    Console.WriteLine("If you wish to stop it, please type the word 'stop' in this console.");

    Console.BackgroundColor = ConsoleColor.Black;

    await Task.Delay(500);

    MinecraftApi api = new(args);
    _ = Task.Run(() => api.StartAsync());
    

    while (true)
    {
        string? text = Console.ReadLine();
        if(text == "stop")
        {
            try
            {
                await api.Stop();
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while stopping the application: " + e.Message);
            }
        }
    }
}
catch(Exception e)
{
    LogService.GetService<WebApiLogger>().LogFatal(e);
}