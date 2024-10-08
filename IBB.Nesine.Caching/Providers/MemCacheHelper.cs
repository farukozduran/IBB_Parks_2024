using IBB.Nesine.Data;
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

        public MemCacheHelper(IDbProvider dbProvider) : base()
        {
            _dbProvider = dbProvider;
        }
    }
}
