using Newtonsoft.Json;
using StackExchange.Redis;

namespace IBB.Nesine.Caching
{
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly IDatabase _db;
        public RedisCacheProvider()
        {
            _db = ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(0);
        }
        public T? Get<T>(string key)
        {
            var value = _db.StringGet(key);
            var deserialize = JsonConvert.DeserializeObject<T>(value);
            return deserialize;
        }
        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var result = JsonConvert.SerializeObject(value);
            _db.StringSet(key, result, expiration);
        }
    }
}
