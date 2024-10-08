using IBB.Nesine.Caching.Interfaces;
using IBB.Nesine.Data;
using Microsoft.Extensions.Caching.Memory;

namespace IBB.Nesine.Caching.Providers
{
    public class MemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCacheProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public T? Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set<T>(key, value, expiration);
        }
    }
}
