using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql
{
    public class MongoDatabaseProvider : IMongoDatabaseProvider
    {
        private readonly MongoUrl _url;
        private readonly MongoClientSettings _settings;

        public MongoDatabaseProvider(IMongoConnectionStringProvider connectionStringProvider)
        {
            _url = new MongoUrl(connectionStringProvider.GetConnectionString());
            _settings = MongoClientSettings.FromUrl(_url);
        }

        public IMongoDatabase GetDatabase()
        {
            return new MongoClient(_settings).GetDatabase(_url.DatabaseName);
        }
    }

}
