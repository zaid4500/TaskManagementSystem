using ManagementBE.Kernel.Core.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementBE.Kernel.Core.Caching
{
    public class LocalCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private TimeSpan expiryTime = new TimeSpan(24, 0, 0, 0);
        public LocalCacheService(IMemoryCache cache ) =>
        (_cache) = (cache);

        public T? Get<T>(string key) =>
        _cache.Get<T>(key);

        public Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
            Task.FromResult(Get<T>(key));

        public string GetCacheKey(string name, object id)
        {
            return $"{name}-{id}";
        }

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

        //public async Task<T> GetObjectAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiry = null) where T : class
        //{
        //    var cachedObject = GetObjectFromCache<T>(key);
        //    if (cachedObject == null)
        //    {
        //        expiryTime = expiry ?? expiryTime;
        //        cachedObject = await UpdateCahceAsync(key, func);
        //    }
        //    return cachedObject;
        //}

        //private T GetObjectFromCache<T>(string key) where T : class
        //{
        //    return _cache.Get<T>(key);
        //}

        //private async Task<T> UpdateCahceAsync<T>(string key, Func<Task<T>> func) where T : class
        //{
        //    T dbObject = await GetObjectFromDataSourceAsync(func);
        //    if (dbObject == null)
        //    {
        //        return null;
        //    }

        //    InsertObjectIntoCache(key, dbObject);
        //    return dbObject;
        //}

        //private async Task<T> GetObjectFromDataSourceAsync<T>(Func<Task<T>> func)
        //{
        //    return await func();
        //}
        //public void InsertObjectIntoCache(string key, object value)
        //{
        //    _cache.Set(key, value, expiryTime);
        //}
    }
}
