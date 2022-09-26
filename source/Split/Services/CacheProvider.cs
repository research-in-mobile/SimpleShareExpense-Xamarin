using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Split.Services
{
    public class CacheProvider : ICacheProvider
    {
        private static MemoryCache _cache;
        private static MemoryCache Cache => _cache ?? (_cache = new MemoryCache(new MemoryCacheOptions()));

        public T Get<T>([CallerMemberName] string key = null) => Cache.Get<T>(key);

        public Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
            => GetOrCreateAsync(key, Constants.DefaultCacheExpiration, factory);

        public Task<T> GetOrCreateAsync<T>(string key, TimeSpan expiration, Func<Task<T>> factory)
        {
            return Cache.GetOrCreateAsync(key, async ce =>
            {
                ce.SetAbsoluteExpiration(expiration);
                var result = await factory.Invoke();
                return result;
            });
        }

        public T GetOrCreate<T>(string key, Func<T> factory)
            => GetOrCreate(key, Constants.DefaultCacheExpiration, factory);

        public T GetOrCreate<T>(string key, TimeSpan expiration, Func<T> factory)
        {
            return Cache.GetOrCreate(key, ce =>
            {
                ce.SetAbsoluteExpiration(expiration);
                var result = factory.Invoke();
                return result;
            });
        }

        public void Set<T>(T value, DateTimeOffset? absoluteExpiry = null, [CallerMemberName] string key = null)
        {
            if (absoluteExpiry is null) absoluteExpiry = DateTimeOffset.Now.Add(Constants.DefaultCacheExpiration);
            _cache.Set(key, value, absoluteExpiry.Value);
        }

        public void Remove(string key) => Cache.Remove(key);

        public void Clear()
        {
            _cache?.Dispose();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }
    }


}
