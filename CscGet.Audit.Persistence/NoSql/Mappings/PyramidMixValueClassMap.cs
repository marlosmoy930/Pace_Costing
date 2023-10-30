using CscGet.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class PyramidMixValueClassMap : MongoDbClassMap<PyramidMixValue>
    {
        protected override void Map(BsonClassMap<PyramidMixValue> cm)
        {
            cm.AutoMap();
            cm.MapProperty(x => x.Value).SetSerializer(new DecimalSerializer(BsonType.Decimal128));
            cm.MapProperty(x => x.YearNumber);
        }
    }
}
