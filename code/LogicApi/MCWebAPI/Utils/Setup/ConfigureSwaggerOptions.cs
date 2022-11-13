using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MCWebAPI.Utils.Setup
{
    /// <summary>
    /// Configures the Swagger options.
    /// </summary>
    internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;


        /// <summary>
        /// Initializes the ConfigureSwaggerOptions.
        /// </summary>
        /// <param name="provider"></param>
        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Configure each API discovered for Swagger Documentation
        /// </summary>
        /// <param name="options"></param>
        public void Configure(SwaggerGenOptions options)
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.IgnoreObsoleteActions();
            options.IgnoreObsoleteProperties();
            options.CustomSchemaIds(type => type.FullName);

            options.AddSignalRSwaggerGen();

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            options.OperationFilter<SecurityRequirementsOperationFilter>(true, "Bearer");
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Standard Authorization header using the Bearer scheme (JWT). Example: \"bearer {token}\"",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            // add swagger document for every API version discovered
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    CreateVersionInfo(description));
            }
        }

        /// <summary>
        /// Configure Swagger Options. Inherited from the Interface
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void Configure(string name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        /// <summary>
        /// Create information about the version of the API
        /// </summary>
        /// <param name="desc"></param>
        /// <returns>Information about the API</returns>
        private static OpenApiInfo CreateVersionInfo(
                ApiVersionDescription desc)
        {
            var info = new OpenApiInfo()
            {
                Title = ".NET Core (.NET 6) Web API",
                Version = desc.ApiVersion.ToString()
            };

            if (desc.IsDeprecated)
            {
                info.Description += " This API version has been deprecated. Please use one of the new APIs available from the explorer.";
            }

            return info;
        }
    }
}
