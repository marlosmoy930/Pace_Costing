using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Domain.Models;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public interface IAuditRecordRepository<TAuditRecord, TValue> where TAuditRecord : AuditRecord<TValue> where TValue : class
    {
        Task<TAuditRecord> GetByIdAsync(Guid id);
        Task<List<TAuditRecord>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task AddOrUpdateAsync(TAuditRecord auditRecord);
        Task AddAsync(IReadOnlyCollection<TAuditRecord> auditRecord);
        Task RemoveAsync(IReadOnlyCollection<Guid> ids);
        Task UpdateAsync(IReadOnlyCollection<TAuditRecord> auditRecords);
    }
}
