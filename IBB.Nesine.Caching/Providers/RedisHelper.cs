using IBB.Nesine.Data;
using StackExchange.Redis;

namespace IBB.Nesine.Caching.Providers
{
    public class RedisHelper : RedisCacheProvider
    {
        private readonly IDbProvider _dbProvider;

        public RedisHelper(IConnectionMultiplexer redisConnection, IDbProvider dbProvider): base(redisConnection)
        {
            _dbProvider = dbProvider;
        }

        public IEnumerable<T> GetData<T>(string cacheKey, TimeSpan expiration, string spName, object parameters)
        {
            var cachedValue = Get<IEnumerable<T>>(cacheKey); // Burada IEnumerable<T> bekleniyor
            if (cachedValue is not null) return cachedValue;

            IEnumerable<T> data;
            if (parameters is not null)
                data = _dbProvider.Query<T>(spName, parameters);
            else
                data = _dbProvider.Query<T>(spName);

            Set(cacheKey, data, expiration);
            return data;
        }

    }
}
