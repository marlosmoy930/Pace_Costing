using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using Dxc.Captn.Costing.Contracts.Quantity;

namespace CscGet.Audit.Application.Services
{
    public interface IQuantityAuditService : IAditRecordService<EntityRecordCopyModel>
    {
        Task CreateOrUpdateExistingRecord(QuantitiesChangedEvent @event);
    }
}
