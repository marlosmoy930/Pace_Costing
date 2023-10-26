using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using Dxc.Captn.Costing.Contracts.Operations.ConvertToCustom;

namespace CscGet.Audit.Application.Services
{
    public interface IConvertToCustomAuditService : IAditRecordService<EntityRecordCopyModel>
    {
        Task CreateRecord(CreateConversionAuditRecordCommand @event, string userName);
    }
}
