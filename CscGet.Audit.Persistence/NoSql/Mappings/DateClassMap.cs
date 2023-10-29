using CscGet.Audit.Domain.Models.Core;
using MongoDB.Bson.Serialization;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class DateClassMap : MongoDbClassMap<Date>
    {
        protected override void Map(BsonClassMap<Date> cm)
        {
            cm.AutoMap();
            cm.MapProperty(x => x.Month);
            cm.MapProperty(x => x.Year);
            cm.MapCreator(x => new Date(x.Year, x.Month));
        }
    }
}
