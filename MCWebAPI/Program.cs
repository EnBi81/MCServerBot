using Application.DAOs;
using Application.Minecraft;
using Application.Permissions;
using DataStorageSQLite.Implementation;
using Loggers;
using MCWebAPI;
using MCWebAPI.Auth;
using MCWebAPI.Middlewares;
using MCWebAPI.WebSocketHandler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.DTOs;
using Shared.Model;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Security.Claims;
using System.Text;


LogService logService = new LogService()
                .SetupLogger<DiscordLogger>()
                .SetupLogger<HamachiLogger>()
                .SetupLogger<MinecraftLogger>()
                .SetupLogger<WebLogger>()
                .SetupLogger<ConfigLogger>()
                .SetupLogger<NetworkLogger>();

LogService.RegisterLogService(logService);

var config = Config.Instance;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (JWT). Example: \"bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
});

builder.Services.AddSingleton(new MinecraftConfig
{
    JavaLocation = config.JavaLocation,
    MaxSumOfDiskSpaceGB = config.MinecraftMaxDiskSpaceGB,
    MinecraftServerHandlerPath = config.MinecraftServerHandlerPath,
    MinecraftServerInitRamMB = config.MinecraftServerInitRamMB,
    MinecraftServerMaxRamMB = config.MinecraftServerMaxRamMB,
    MinecraftServersBaseFolder = config.MinecraftServersBaseFolder
});

builder.Services.AddSingleton<IDatabaseAccess, DataStorageSQLiteImpl>();
builder.Services.AddSingleton<IServerPark, ServerPark>();
builder.Services.AddSingleton<IPermissionLogic, PermissionLogic>();


builder.Services.AddSingleton<SocketPool>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("DiscordBot", a =>
        a.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, DataUserType.Discord.ToString()));
});


var app = builder.Build();

await app.Services.GetRequiredService<IDatabaseAccess>().DatabaseSetup.Setup("Data Source=C:\\Users\\enbi8\\source\\repos\\MCServerBot\\MCWebApp\\Resources\\eventdata.db;Version=3;");
await app.Services.GetRequiredService<IServerPark>().InitializeAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(cors => cors
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<MCExceptionHandlerMiddleware>();

app.Run();
