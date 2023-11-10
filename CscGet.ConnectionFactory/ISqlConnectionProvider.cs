using System;
using System.Data.SqlClient;

namespace CscGet.ConnectionFactory
{
    public interface ISqlConnectionProvider : IDisposable
    {
        SqlConnection GetConnection();

        string GetConnectionString();
    }
}
