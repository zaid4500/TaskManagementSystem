using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Host.Base.Configurations.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema.Generation.TypeMappers;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using System.Linq;

namespace ManagementBE.Kernel.Host.Base.ServiceRegistration
{
    public static class SwaggerRegistration
    {
        /// <summary>
        /// Register Swagger 
        /// </summary>
        /// <param name="services"></param>
        internal static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, Configuration config)
        {
            var settings = config.SwaggerSettings;
            if (settings.Enable)
            {
                services.AddVersionedApiExplorer(o => o.SubstituteApiVersionInUrl = true);
                services.AddEndpointsApiExplorer();
                services.AddOpenApiDocument((document, serviceProvider) =>
                {
                    document.PostProcess = doc =>
                    {
                        doc.Info.Title = settings.Title;
                        doc.Info.Version = settings.Version;
                    };

                    document.AddSecurity(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Input your Bearer token to access this API",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT",
                    });

                    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
                    document.OperationProcessors.Add(new SwaggerGlobalAuthProcessor());

                    document.TypeMappers.Add(new PrimitiveTypeMapper(typeof(TimeSpan), schema =>
                    {
                        schema.Type = NJsonSchema.JsonObjectType.String;
                        schema.IsNullableRaw = true;
                        schema.Pattern = @"^([0-9]{1}|(?:0[0-9]|1[0-9]|2[0-3])+):([0-5]?[0-9])(?::([0-5]?[0-9])(?:.(\d{1,9}))?)?$";
                        schema.Example = "02:00:00";
                    }));

                    document.OperationProcessors.Add(new SwaggerHeaderAttributeProcessor());

                    document.SchemaProcessors.Add(new SwaggerGuidSchemaProcessor());

                    document.PostProcess = d =>
                    {
                        var pathsToRemove = d.Paths
                            .Where(pair => !pair.Key.StartsWith("/api/v1"))
                            .Select(pair => pair.Key)
                            .ToList();

                        foreach (var pathToRemove in pathsToRemove)
                        {
                            d.Paths.Remove(pathToRemove);
                        }
                    };
                });
            }
            return services;
        }

        internal static IServiceCollection AddOpenApiVersioning(this IServiceCollection services) =>
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

        internal static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app, Configuration config)
        {
            if (config.SwaggerSettings.Enable)
            {
                app.UseOpenApi();
                app.UseSwaggerUi3(options =>
                {
                    options.DefaultModelsExpandDepth = -1;
                    options.DocExpansion = "none";
                    options.TagsSorter = "alpha";
                });
            }

            return app;
        }
    }
}
