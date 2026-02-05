//using Microsoft.Extensions.DependencyInjection;
//using ManagementBE.Kernel.Core.Configurations;
//using ManagementBE.Kernel.Core.Infrastructure;
//using ManagementBE.Kernel.Infrastructure.Logging.Loggers;
//using ManagementBE.Kernel.Infrastructure.Persistence.Repositories;

//namespace ManagementBE.Kernel.Infrastructure.Logging
//{
//    /// <summary>
//    /// Service Registration
//    /// </summary>
//    public static class ServiceRegistration
//    {
//        /// <summary>
//        /// Add Application Loggers
//        /// </summary>
//        /// <param name="services"></param>
//        /// <returns></returns>
//        public static IServiceCollection AddApplicationLoggers(this IServiceCollection services, Configuration config)
//        {
//            services.AddScoped<IApplicationLogRepository, ApplicationLogRepository>();
//            services.AddScoped<IApplicationLoggerService, ApplicationDbLogger>();

//            if (config.ExceptionLogEnabled)
//            {
//                services.AddScoped<ExceptionFilter, ExceptionFilter>();
//                services.AddMvc(options => options.Filters.AddService(typeof(ExceptionFilter)));
//            }

//            if (config.HttpLogEnabled)
//            {
//                services.AddScoped<HttpLogger>();
//                services.AddMvc(options => options.Filters.AddService(typeof(HttpLogger)));
//            }
//            return services;
//        }
//    }
//}
