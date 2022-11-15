using Loggers;
using Loggers.Loggers;
using MCWebAPI;
using System.Reflection;

Console.WriteLine("Calling: " + Assembly.GetCallingAssembly()?.Location);
Console.WriteLine("Executing: " + Assembly.GetExecutingAssembly()?.Location);
Console.WriteLine("Entry: " + Assembly.GetEntryAssembly()?.Location);

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
        else if(!string.IsNullOrWhiteSpace(text))
        {
            Console.WriteLine("To stop the program, please type 'stop'.");
        }
    }
}
catch(Exception e)
{
    LogService.GetService<WebApiLogger>().LogFatal(e);
}