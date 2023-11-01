using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.Constants;
using CscGet.Audit.Persistence.NoSql.Initializers;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public class ConvertToCustomRepository : AuditRecordRepositoryBase<ConvertToCustomAuditRecord, string>
    {
        public ConvertToCustomRepository(IMongoDatabaseProvider mongoDatabaseProvider, IMongoCollectionInitializer<ConvertToCustomAuditRecord> mongoCollectionInitializer) : base(mongoDatabaseProvider, mongoCollectionInitializer)
        {
        }

        protected override string CollectionName => CollectionNames.ConvertToCustomAuditLog;
    }
}
