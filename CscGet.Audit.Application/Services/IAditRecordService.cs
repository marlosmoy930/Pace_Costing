using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;

namespace CscGet.Audit.Application.Services
{
    public interface IAditRecordService<TRecordCopyModel> where TRecordCopyModel : EntityRecordCopyModel
    {
        Task CopyRecordsAsync(int targetCostingVersionId, IReadOnlyCollection<TRecordCopyModel> recordCopyModels);

        Task RemoveRecordsAsync(Guid[] ids);
    }
}
