using IBB.Nesine.Caching.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace IBB.Nesine.Caching.Providers
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IDatabase _redisDb;
        public RedisCacheProvider(IConnectionMultiplexer redisConnection)
        {
            _redisDb = redisConnection.GetDatabase();
        }

        public T Get<T>(string key)
        {
            var value = _redisDb.StringGet(key);
            if (!value.HasValue) return default;
            return JsonConvert.DeserializeObject<T>(value);
        }
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var result = JsonConvert.SerializeObject(value);
            _redisDb.StringSet(key, result, expiration);
        }
    }
}
