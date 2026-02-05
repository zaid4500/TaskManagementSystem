using ManagementBE.Kernel.Core;
using ManagementBE.Kernel.Core.Common;
using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Persistence;
using ManagementBE.Kernel.Host.Base.Configurations.Auth;
using ManagementBE.Kernel.Host.Base.Configurations.Logging;
using ManagementBE.Kernel.Host.Base.Middlewares;
using ManagementBE.Kernel.Host.Base.ServiceRegistration;
using ManagementBE.Kernel.Infrastructure.Persistence.Initialization;
using ManagementBE.Kernel.Infrastructure.Persistence.Repositories;
using ManagementBE.Kernel.Infrastructure.Persistence.Services;
using ManagementBE.Kernel.Infrastructure.Persistence.UnitOfWorks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ManagementBE.Kernel.Host.Base
{
    public static class ApplicationBuilderExtensions
    {

        public static IServiceCollection BuildMainServices(this IServiceCollection services)
        {
            services
                .AddSingleton(Log.Logger)
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped<ICrossCuttingDependencies, CrossCuttingDependencies>()
                .AddTransient<ICacheService, CacheService>()
                .AddFluentValidationRegistration()
                .RegisterRunningModuleDependencies();
            return services;
        }

        public static TConfiguration ConfigureServices<TConfiguration>(this WebApplicationBuilder builder)
        where TConfiguration : Configuration
        {
            var env = builder.Environment;

            //Configuration json files
            var config = BuildConfiguration<TConfiguration>(env);

            // Main Services
            builder.Services.AddSingleton<Configuration>(config);


            builder.Services.BuildMainServices();

            //HelperServices
            builder.Services.AddControllers();
            builder.Services.AddControllers().AddNewtonsoftJson();
            builder.Services.AddControllers().AddXmlSerializerFormatters();

            builder.Services.AddOpenApiVersioning();
            builder.Services.AddAuth(config);
            builder.Services.AddCaching(config);


            builder.Services.AddHttpLoggingMiddleware();
            builder.Services.AddOpenApiDocumentation(config);
            builder.Services.DbContextRegistration(config);


            return config;
        }

        private static TConfiguration BuildConfiguration<TConfiguration>(IWebHostEnvironment env)
        where TConfiguration : Configuration
        {
            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sharedsettings.json"), true, true)
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sharedsettings.Development.json"), true, true)
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), true, true)
                .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.{Environment}.json"), true, true)
                .AddEnvironmentVariables();

            var configRoot = builder.Build();
            return configRoot.Get<TConfiguration>();
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, Configuration config)
        {

            builder
                .UseRequestLocalization()
                .UseStaticFiles()
                .UseHttpLoggingMiddleware()
                .UseRouting()
                .UseCors("CorsPolicy")
                .UseCorsMiddleware()
                .UseAuthentication()
                .UseCurrentUser()
                .UseAuthorization()
                .UseOpenApiDocumentation(config)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            return builder;
        }

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapControllers().RequireAuthorization();

            return builder;
        }

        static void DbContextRegistration(this IServiceCollection services, Configuration config)
        {
            services.AddDbContext<IUnitOfWork, UnitOfWork>(options => options.UseInMemoryDatabase("ManagementDB"))
            //services.AddDbContext<IUnitOfWork, UnitOfWork>(options => options.UseSqlServer(
            //    config.ConnectionStrings.Default,
            //   providerOptions => providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null).CommandTimeout(60)).EnableSensitiveDataLogging(true), ServiceLifetime.Transient)

                .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
                .AddTransient<ApplicationDbInitializer>()
                .AddTransient<ApplicationDbSeeder>()
                .AddServices(typeof(ICustomSeeder), ServiceLifetime.Transient)
                .AddTransient<CustomSeederRunner>();
        }

        public static void ConfigureCommonServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCorsPolicyRegistration();
            builder.Services.AddFluentValidationRegistration();
            builder.Services.RegisterRunningModuleDependencies();
        }
    }
}
