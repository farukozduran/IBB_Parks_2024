using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBB.Nesine.Data
{
    public interface IDbProvider
    {
        public IEnumerable<T> Query<T>(string storedProcName, object parameters);
        public IEnumerable<T> Query<T>(string storedProcName);
        public int Execute(string storedProcName, object parameters);
    }
}
