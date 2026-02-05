using ManagementBE.Kernel.Core.Infrastructure;
using ManagementBE.Kernel.Core.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ManagementBE.Kernel.Infrastructure.Persistence.Services
{
    public class CacheService : ICacheService, IApplicationService
    {
        readonly IMemoryCache _cache;
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T? Get<T>(string key) =>
        _cache.Get<T>(key);

        public Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
        Task.FromResult(Get<T>(key));

        public void Refresh(string key) =>
            _cache.TryGetValue(key, out _);

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            Refresh(key);
            return Task.CompletedTask;
        }

        public void Remove(string key) =>
            _cache.Remove(key);

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
        {
            
            slidingExpiration ??= TimeSpan.FromMinutes(10); 

            _cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration });
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
        {
            Set(key, value, slidingExpiration);
            return Task.CompletedTask;
        }

        public string GetCacheKey(string name, object id)
        {
            return $"{name}-{id}";
        }
    }


    public static class CacheServiceExtensions
    {
        public static T? GetOrSet<T>(this ICacheService cache, string key, Func<T?> getItemCallback, TimeSpan? slidingExpiration = null)
        {
            T? value = cache.Get<T>(key);

            if (value is not null)
            {
                return value;
            }

            value = getItemCallback();

            if (value is not null)
            {
                cache.Set(key, value, slidingExpiration);
            }

            return value;
        }

        public static async Task<T?> GetOrSetAsync<T>(this ICacheService cache, string key, Func<Task<T>> getItemCallback, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default)
        {
            T? value = await cache.GetAsync<T>(key, cancellationToken);

            if (value is not null)
            {
                return value;
            }

            value = await getItemCallback();

            if (value is not null)
            {
                await cache.SetAsync(key, value, slidingExpiration, cancellationToken);
            }

            return value;
        }
    }
}
