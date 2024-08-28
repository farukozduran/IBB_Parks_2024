using System.Collections.Generic;

namespace IBB.Nesine.Data
{
    public interface IDbProvider
    {
        public IEnumerable<T> Query<T>(string storedProcName, object parameters);
        public IEnumerable<T> Query<T>(string storedProcName);
        public int Execute(string storedProcName, object parameters);
        public T QuerySingle<T>(string storedProcName, object parameters);
    }
}
