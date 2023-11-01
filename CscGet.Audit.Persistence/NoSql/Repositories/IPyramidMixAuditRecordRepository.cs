using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Domain.Models;

namespace CscGet.Audit.Persistence.NoSql.Repositories
{
    public interface IPyramidMixAuditRecordRepository : IAuditRecordRepository<PyramidMixAuditRecord, PyramidMixValue[]>
    {
        Task<List<PyramidMixAuditRecord>> GetByCostGroupIdAsync(Guid costGroupId);
        Task RemoveByCostGroupIdsAsync(params Guid[] costGroupIds);
    }
}