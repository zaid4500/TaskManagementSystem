using ManagementBE.Kernel.Core.Caching;
using ManagementBE.Kernel.Core.Configurations;
using ManagementBE.Kernel.Core.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace ManagementBE.Kernel.Host.Base.ServiceRegistration
{
    internal static class CachingRegistration
    {
        internal static IServiceCollection AddCaching(this IServiceCollection services, Configuration config)
        {
            var settings = config.CacheSettings;
            if (settings.UseDistributedCache)
            {
                if (settings.PreferRedis)
                {
                    
                }
                else
                {
                    services.AddDistributedMemoryCache();
                }

                services.AddTransient<ICacheService, DistributedCacheService>();
            }
            else
            {
                services.AddTransient<ICacheService, LocalCacheService>();
            }

            services.AddMemoryCache();
            return services;
        }
    }
}
