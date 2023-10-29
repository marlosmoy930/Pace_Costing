using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql
{
    public interface IMongoDatabaseProvider
    {
        IMongoDatabase GetDatabase();
    }
}