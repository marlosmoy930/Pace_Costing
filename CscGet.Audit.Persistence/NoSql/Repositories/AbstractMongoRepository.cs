using CscGet.Audit.Persistence.NoSql.Initializers;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public abstract class AbstractMongoRepository<TDocument>
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollectionInitializer<TDocument> _mongoCollectionInitializer;

        protected AbstractMongoRepository(IMongoDatabaseProvider mongoDatabaseProvider, IMongoCollectionInitializer<TDocument> mongoCollectionInitializer)
        {
            _database = mongoDatabaseProvider.GetDatabase();
            _mongoCollectionInitializer = mongoCollectionInitializer;
        }

        protected abstract string CollectionName { get; }

        protected IMongoCollection<TDocument> GetCollection()
        {
            var collectionSettings =
                new MongoCollectionSettings
                {
                    GuidRepresentation = _mongoCollectionInitializer.GetGuidRepresentation()
                };

            var collection = _database.GetCollection<TDocument>(CollectionName, collectionSettings);

            var indexes = _mongoCollectionInitializer.CreateIndexes();
            if (indexes != null)
                collection.Indexes.CreateMany(indexes);
            return collection;
        }
    }
}