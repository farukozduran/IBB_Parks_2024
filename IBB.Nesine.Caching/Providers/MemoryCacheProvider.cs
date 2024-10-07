using IBB.Nesine.Caching.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace IBB.Nesine.Caching.Providers
{
    public class MemoryCacheProvider
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        //bool ICacheProvider.Any(string key)
        //{
        //    return _memoryCache.Get(key) != null;
        //}
        public T? Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
        void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set<T>(key, value, expiration);
        }

    }
}
