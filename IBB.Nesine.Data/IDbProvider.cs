using System.Collections.Generic;

namespace IBB.Nesine.Data
{
    public interface IDbProvider
    {
        IEnumerable<T> Query<T>(string storedProcName, object parameters);
        IEnumerable<T> Query<T>(string storedProcName);
        //public Task<IEnumerable<T>> QueryAsync<T>(string sql);
        int Execute(string storedProcName, object parameters);
        int Execute(string storedProcName);

        T QuerySingle<T>(string storedProcName, object parameters);
        int ExecuteSql(string sql, object parameters);
        T ExecuteScalarAsync<T>(string sql, object parameters);

    }
}
