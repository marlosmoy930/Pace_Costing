using CscGet.Audit.Domain.Models;
using MongoDB.Bson.Serialization;

namespace CscGet.Audit.Persistence.NoSql.Mappings
{
    public class PyramidMixAuditRecordClassMap : MongoDbClassMap<PyramidMixAuditRecord>
    {
        protected override void Map(BsonClassMap<PyramidMixAuditRecord> cm)
        {
            cm.AutoMap();
            cm.MapProperty(x => x.CostGroupId);
            cm.MapCreator(x => new PyramidMixAuditRecord(x.Id, x.CostingVersionId, x.ModificationDate, x.UserId, x.ModifiedBy, x.TemplateValue, x.CurrentValue, x.Reason, x.CostGroupId));
        }
    }
}
