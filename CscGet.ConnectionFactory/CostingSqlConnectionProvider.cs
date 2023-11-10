using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using AsyncFriendlyStackTrace;
using CscGet.ConfigManager.ServiceSettings;
using Dxc.Captn.Infrastructure.Settings.Sql.Configuration;
using Serilog;

namespace CscGet.ConnectionFactory
{
    public class CostingSqlConnectionProvider : ISqlConnectionProvider, IDisposable
    {
        private readonly string _connectionString;
        private readonly ConcurrentBag<SqlConnection> _connections = new ConcurrentBag<SqlConnection>();

        public CostingSqlConnectionProvider(SqlConnectionStringsFactory connectionStringsFactory)
        {
            _connectionString = connectionStringsFactory.Get(ConnectionStringsConstants.RelationDbKey);
        }

        public SqlConnection GetConnection()
        {
            var connection = GetOpenedConnection();

            _connections.Add(connection);

            return connection;
        }

        public string GetConnectionString() => _connectionString;

        public void Dispose()
        {
            var count = _connections.Count;
            while (_connections.TryTake(out var sqlConnection))
            {
                try
                {
                    using (sqlConnection)
                    {
                        if (sqlConnection?.State == ConnectionState.Open)
                            sqlConnection.Close();
                    }
                }
                catch (Exception /*e*/)
                {
                    //Log.Error("SqlConnectionProvider Dispose exception {0} {1}", e.ToAsyncString(), e);
                }
            }

            //Log.Information("SqlConnectionProvider Dispose count {1}", count);
        }

        private SqlConnection GetOpenedConnection()
        {
            int retryCount = 10;

            while (true)
            {
                try
                {
                    var connection = new SqlConnection(_connectionString);
                    connection.Open();
                    return connection;
                }
                catch (Exception e)
                {
                    Log.Error("SqlConnectionProvider Error {0} {1} {2} {3}", e.ToAsyncString(), e.InnerException?.ToAsyncString(), e, e.InnerException);
                    Thread.Sleep(1000);
                    if (retryCount-- == 0)
                        throw;
                }
            }
        }
    }
}
