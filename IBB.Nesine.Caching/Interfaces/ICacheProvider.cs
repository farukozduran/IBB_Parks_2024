using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBB.Nesine.Caching.Interfaces
{
    public interface ICacheProvider
    {
        //bool Any(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
        //T GetData<T>(string cacheKey, TimeSpan expiration, string spName, object parameters);
    }
}
