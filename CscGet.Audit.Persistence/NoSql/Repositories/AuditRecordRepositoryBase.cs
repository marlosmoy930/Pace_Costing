using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.NoSql.Initializers;
using MongoDB.Driver;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public abstract class AuditRecordRepositoryBase<TAuditRecord, TValue> : AbstractMongoRepository<TAuditRecord>, IAuditRecordRepository<TAuditRecord, TValue> where TAuditRecord : AuditRecord<TValue> where TValue : class
    {
        protected AuditRecordRepositoryBase(IMongoDatabaseProvider mongoDatabaseProvider, IMongoCollectionInitializer<TAuditRecord> mongoCollectionInitializer) : base(mongoDatabaseProvider, mongoCollectionInitializer)
        {
        }

        public Task<TAuditRecord> GetByIdAsync(Guid id)
        {
            var collection = GetCollection();
            return collection.Find(x => x.Id == id).SingleOrDefaultAsync();
        }

        public Task<List<TAuditRecord>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var idsSet = new HashSet<Guid>(ids);
            var collection = GetCollection();
            var query = collection.Find(x => idsSet.Contains(x.Id));
            return query.ToListAsync();
        }

        public Task AddOrUpdateAsync(TAuditRecord auditRecord)
        {
            var collection = GetCollection();
            return collection.ReplaceOneAsync(x => x.Id == auditRecord.Id, auditRecord, new UpdateOptions {IsUpsert = true});
        }

        public Task AddAsync(IReadOnlyCollection<TAuditRecord> auditRecords)
        {
            var collection = GetCollection();
            return collection.InsertManyAsync(auditRecords);
        }

        public Task UpdateAsync(IReadOnlyCollection<TAuditRecord> auditRecords)
        {
            var writeModels = new List<WriteModel<TAuditRecord>>();
            foreach (var auditRecord in auditRecords)
            {
                var id = auditRecord.Id;
                var model = new ReplaceOneModel<TAuditRecord>(new ExpressionFilterDefinition<TAuditRecord>(y => y.Id == id), auditRecord);
                writeModels.Add(model);
            }

            var collection = GetCollection();
            return collection.BulkWriteAsync(writeModels, new BulkWriteOptions {IsOrdered = true});
        }

        public Task RemoveAsync(IReadOnlyCollection<Guid> ids)
        {
            var idsSet = new HashSet<Guid>(ids);
            var collection = GetCollection();
            return collection.DeleteManyAsync(x => idsSet.Contains(x.Id));
        }
    }
}
