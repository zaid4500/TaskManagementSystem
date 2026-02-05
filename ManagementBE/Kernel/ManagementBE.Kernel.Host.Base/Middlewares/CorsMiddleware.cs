using Microsoft.AspNetCore.Builder;

namespace ManagementBE.Kernel.Host.Base.Middlewares
{
    public static class CorsMiddleware
    {
        /// <summary>
        /// Used for API's security  
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder app)
        {
            app.UseCors(x => x
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .SetIsOriginAllowed(origin => true)
                 .AllowCredentials());
            return app;
        }
    }
}
