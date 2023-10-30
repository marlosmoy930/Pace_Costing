using CscGet.Audit.Domain.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class QuantityClassMap : MongoDbClassMap<Quantity>
    {
        protected override void Map(BsonClassMap<Quantity> cm)
        {
            cm.AutoMap();
            cm.MapCreator(x => new Quantity(x.Value, x.Date.Month, x.Date.Year, x.Formula));
            cm.MapProperty(x => x.Date);
            cm.MapProperty(x => x.Formula);
            cm.MapProperty(x => x.Value).SetSerializer(new DecimalSerializer(BsonType.Decimal128));
        }
    }
}
