using MongoDB.Bson.Serialization;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public abstract class MongoDbClassMap<T> : IMongoDbClassMap
    {
        protected abstract void Map(BsonClassMap<T> cm);

        public void Map()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(T)))
                return;

            BsonClassMap.RegisterClassMap<T>(Map);
        }
    }
}
