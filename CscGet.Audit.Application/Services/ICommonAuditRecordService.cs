using CscGet.Audit.Application.Models;

namespace CscGet.Audit.Application.Services
{
    public interface ICommonAuditRecordService<TAuditRecord, TValue> : IAditRecordService<EntityRecordCopyModel>
    {
    }
}
