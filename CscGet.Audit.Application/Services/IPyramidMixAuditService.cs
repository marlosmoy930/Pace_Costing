using System;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Costing.Domain.Dispatcher.Events.LaborRates;

namespace CscGet.Audit.Application.Services
{
    public interface IPyramidMixAuditService : IAditRecordService<ElementRecordCopyModel>
    {
        Task CreateOrUpdateRecordsAsync(LaborRatePyramidMixValuesChangedEvent @event);

        Task RemoveRecordsForCostGroupsAsync(Guid[] costGroupIds);
    }
}