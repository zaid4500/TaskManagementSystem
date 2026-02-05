using Microsoft.AspNetCore.Builder;

namespace ManagementBE.Kernel.Host.Base.Middlewares
{
    public static class StaticFilesMiddleware
    {
        /// <summary>
        /// Used for Static Files in the system 
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseStaticFilesMiddleware(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/yaml"
            });

            return app;
        }
    }
}
