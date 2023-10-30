using CscGet.Audit.Domain.Models;
using MongoDB.Bson.Serialization;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class QuantitiesAuditRecordClassMap : MongoDbClassMap<QuantitiesAuditRecord>
    {
        protected override void Map(BsonClassMap<QuantitiesAuditRecord> cm)
        {
            cm.AutoMap();
            cm.MapCreator(x => new QuantitiesAuditRecord(x.Id, x.GroupType, x.CostingVersionId, x.ModificationDate, x.UserId, x.ModifiedBy, x.TemplateValue, x.CurrentValue, x.Reason));
        }
    }
}
