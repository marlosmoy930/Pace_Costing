using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.Constants;
using CscGet.Audit.Persistence.NoSql.Initializers;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public class GlobalTotalEntityAuditRepository : AuditRecordRepositoryBase<GlobalTotalEntityAuditRecord, string>
    {
        public GlobalTotalEntityAuditRepository(IMongoDatabaseProvider mongoDatabaseProvider, IMongoCollectionInitializer<GlobalTotalEntityAuditRecord> mongoCollectionInitializer) : base(mongoDatabaseProvider, mongoCollectionInitializer)
        {
        }

        protected override string CollectionName => CollectionNames.GlobalTotalEntitiesAuditLog;
    }
}
