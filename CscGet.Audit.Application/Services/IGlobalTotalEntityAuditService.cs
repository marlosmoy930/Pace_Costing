using System.Threading.Tasks;
using CscGet.Audit.Application.Models;

namespace CscGet.Audit.Application.Services
{
    public interface IGlobalTotalEntityAuditService : IAditRecordService<EntityRecordCopyModel>
    {
        Task AddOrUpdateRecordAsync(GlobalTotalEntityRecordModel model);
    }
}