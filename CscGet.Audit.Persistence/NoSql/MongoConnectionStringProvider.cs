using CscGet.ConfigManager.ServiceSettings;
using Dxc.Captn.Infrastructure.Settings.Mongo.Configuration;

namespace CscGet.Audit.Persistence.NoSql
{
    public class MongoConnectionStringProvider : IMongoConnectionStringProvider
    {
        private readonly string _auditMongoDbConnectionString;

        public MongoConnectionStringProvider(MongoConnectionStringsFactory connectionStringsFactory)
        {
            _auditMongoDbConnectionString = connectionStringsFactory.Get(ConnectionStringsConstants.AuditMongoDbKey); ;
        }

        public string GetConnectionString()
        {
            return _auditMongoDbConnectionString;
        }
    }
}
