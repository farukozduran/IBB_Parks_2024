using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBB.Nesine.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
    }
}
