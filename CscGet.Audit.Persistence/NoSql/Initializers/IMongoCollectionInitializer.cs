using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Initializers
{
    public interface IMongoCollectionInitializer<TDocument>
    {
        IEnumerable<CreateIndexModel<TDocument>> CreateIndexes();
        GuidRepresentation GetGuidRepresentation();
    }
}
