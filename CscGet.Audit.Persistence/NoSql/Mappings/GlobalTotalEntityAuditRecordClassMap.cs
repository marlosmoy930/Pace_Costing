using CscGet.Audit.Domain.Models;
using MongoDB.Bson.Serialization;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class GlobalTotalEntityAuditRecordClassMap : MongoDbClassMap<GlobalTotalEntityAuditRecord>
    {
        protected override void Map(BsonClassMap<GlobalTotalEntityAuditRecord> cm)
        {
            cm.AutoMap();
            cm.MapCreator(x => new GlobalTotalEntityAuditRecord(x.Id, x.GroupType, x.CostingVersionId, x.ModificationDate, x.UserId, x.ModifiedBy, x.TemplateValue, x.CurrentValue));
        }
    }
}
