using IBB.Nesine.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBB.Nesine.Caching.Providers
{
    public class MemCacheHelper : MemoryCacheProvider
    {
        private readonly IDbProvider _dbProvider;

        public MemCacheHelper(IDbProvider dbProvider, IMemoryCache memoryCache): base(memoryCache)
        {
            _dbProvider = dbProvider;
        }

        public IEnumerable<T> GetData<T>(string cacheKey, TimeSpan expiration, string spName, object parameters)
        {
            var cachedValue = Get<IEnumerable<T>>(cacheKey);
            if(cachedValue is not null) return cachedValue;

            IEnumerable<T> data;
            if(parameters is not null)
                data = _dbProvider.Query<T>(spName, parameters);
            else
                data = _dbProvider.Query<T>(spName);

            Set(cacheKey, data, expiration);
            return data;
        }
    }
}
