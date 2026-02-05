using ManagementBE.Kernel.Host.Base.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Host.Base.Configurations.Logging
{
    public static class HttpLoggingResgistration
    {
        public static IServiceCollection AddHttpLoggingMiddleware(this IServiceCollection services) =>
        services.AddScoped<RequestResponseLoggerMiddleware>();

        public static IApplicationBuilder UseHttpLoggingMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestResponseLoggerMiddleware>();
    }
}
