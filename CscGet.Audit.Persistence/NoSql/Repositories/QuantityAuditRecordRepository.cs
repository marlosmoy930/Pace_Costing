using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.Constants;
using CscGet.Audit.Persistence.NoSql.Initializers;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public class QuantityAuditRecordRepository : AuditRecordRepositoryBase<QuantitiesAuditRecord, Quantity[]>
    {
        public QuantityAuditRecordRepository(IMongoDatabaseProvider mongoDatabaseProvider, IMongoCollectionInitializer<QuantitiesAuditRecord> mongoCollectionInitializer) : base(mongoDatabaseProvider, mongoCollectionInitializer)
        {
        }

        protected override string CollectionName => CollectionNames.QuantitiesAuditLog;
    }
}
