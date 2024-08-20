using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace IBB.Nesine.Data
{
    public class DbProvider : IDisposable, IDbProvider
    {
        private string _connectionString;
        private readonly IConfiguration _configuration;
        public DbProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration["ConnectionString:DefaultConnection"];
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new Exception("No connection string name given");
            }
        }

        private IDbConnection GetDbConnection()
        {
            SqlConnection obj = new SqlConnection(_connectionString) ?? throw new Exception("No connection established");
            obj.Open();
            return obj;
        }

        private CommandDefinition GetCommandDefinition(string storedProcName, object parameters)
        {
            if (string.IsNullOrWhiteSpace(storedProcName))
            {
                throw new Exception("No Stored Procedure name given");
            }

            return new CommandDefinition(storedProcName, parameters, null, commandType: CommandType.StoredProcedure);
        }

        private CommandDefinition GetCommandDefinition(string storedProcName)
        {
            if (string.IsNullOrWhiteSpace(storedProcName))
            {
                throw new Exception("No Stored Procedure name given");
            }

            return new CommandDefinition(storedProcName, null, null, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<T> Query<T>(string storedProcName, object parameters)
        {
            using IDbConnection cnn = GetDbConnection();
            return cnn.Query<T>(GetCommandDefinition(storedProcName, parameters));
        }
        public IEnumerable<T> Query<T>(string storedProcName)
        {
            using IDbConnection cnn = GetDbConnection();
            return cnn.Query<T>(GetCommandDefinition(storedProcName));
        }
        public int Execute(string storedProcName, object parameters)
        {
            using IDbConnection cnn = GetDbConnection();
            return cnn.Execute(GetCommandDefinition(storedProcName, parameters));
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
