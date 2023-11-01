using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.Constants;
using CscGet.Audit.Persistence.NoSql.Initializers;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public class PyramidMixAuditRecordRepository : AuditRecordRepositoryBase<PyramidMixAuditRecord, PyramidMixValue[]>, IPyramidMixAuditRecordRepository
    {
        public PyramidMixAuditRecordRepository(IMongoDatabaseProvider mongoDatabaseProvider, IMongoCollectionInitializer<PyramidMixAuditRecord> mongoCollectionInitializer) : base(mongoDatabaseProvider, mongoCollectionInitializer)
        {
        }

        protected override string CollectionName => CollectionNames.PyramidMixValuesAuditLog;

        public Task<List<PyramidMixAuditRecord>> GetByCostGroupIdAsync(Guid costGroupId)
        {
            var collection = GetCollection();
            var query = collection.Find(x => x.CostGroupId == costGroupId);
            return query.ToListAsync();
        }

        public Task RemoveByCostGroupIdsAsync(params Guid[] costGroupIds)
        {
            var costGroupIdsSet = new HashSet<Guid>(costGroupIds);
            var collection = GetCollection();
            return collection.DeleteManyAsync(x => costGroupIdsSet.Contains(x.CostGroupId));
        }
    }
}
