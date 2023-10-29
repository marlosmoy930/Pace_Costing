using CscGet.Audit.Domain.Models;
using CscGet.Audit.Domain.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class AuditRecordClassMap<T> : MongoDbClassMap<AuditRecord<T>> where T : class
    {
        protected override void Map(BsonClassMap<AuditRecord<T>> cm)
        {
            cm.AutoMap();
            cm.MapProperty(x => x.ModificationDate);
            cm.MapProperty(x => x.CostingVersionId);
            cm.MapProperty(x => x.CurrentValue);
            cm.MapProperty(x => x.Reason);
            cm.MapProperty(x => x.TemplateValue);
            cm.MapProperty(x => x.UserId);
            cm.MapProperty(x => x.GroupType).SetSerializer(new EnumSerializer<GroupType>(BsonType.String));
        }
    }
}
