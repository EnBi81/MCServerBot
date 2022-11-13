using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Options;
using SignalRSwaggerGen.Attributes;
using System.Reflection;

namespace MCWebAPI.Utils.Setup
{
    /// <summary>
    /// Extension methods for the WebApplication
    /// </summary>
    public static class WebAppExtensions
    {
        /// <summary>
        /// Maps all the hubs to the specified route if a RouteAttribute is applied to the hub.
        /// </summary>
        /// <param name="app"></param>
        public static void MapHubs(this IEndpointRouteBuilder app)
        {
            var hubs = Assembly.GetExecutingAssembly().DefinedTypes.Where(type => type.BaseType == typeof(Hub)).ToList();

            if (!hubs.Any())
                return;

            MethodInfo method = typeof(HubEndpointRouteBuilderExtensions)
                        .GetMethods()
                        .Where(m => m.IsGenericMethod)
                        .Where(m => m.Name == nameof(HubEndpointRouteBuilderExtensions.MapHub))
                        .Where(m => m.GetParameters().Length == 2)
                        .First();
            

            foreach (var hubType in hubs)
            {
                var route = hubType.GetCustomAttribute<SignalRHubAttribute>();
                string? path = route?.Path;

                if (path is not null)
                {
                    if (!path.StartsWith("/"))
                        path = "/" + path;

                    MethodInfo generic = method.MakeGenericMethod(hubType);
                    generic.Invoke(app, new object[] { app, path });
                }
            }
        }
    }
}
