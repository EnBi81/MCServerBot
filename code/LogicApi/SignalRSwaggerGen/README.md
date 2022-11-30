## Info

This is a modified SignalRSwaggerGen project. 

Modifications: 
 - adding signalr listeners to the swagger
 - adding signalr functionality to both sender and listeners of a hub

For the original project, please visit: https://github.com/Dorin-Mocan/SignalRSwaggerGen/wiki


## Usage


Add the SignalR functionality to the swagger ui
```cs
WebApplication app = builder.Build();

// this line is really really really important
app.UseSwaggerUI(options => options.AddSignalRFunctionality());

// ... other codes ...

// this discovers the hubs in the project and maps them automatically
app.MapHubs();
```

Also dont forget to add the SignalR to the ServiceCollection
```cs
serviceCollection.AddSignalR();
```


## How it works (Swagger GUI part)

When calling MapHubs, the code adds the /swagger/extensions/SignalRSwaggerSetup and /swagger/extensions/SignalRSwaggerExtension GET endpoints to the WebApplications. These both contains a javascript code which will be injected into the swagger page, and will take care of the rest :D

If you are interested in the javascript code (and a little css), please check out the Repo/Tools/signalRSwagger/ folder.
