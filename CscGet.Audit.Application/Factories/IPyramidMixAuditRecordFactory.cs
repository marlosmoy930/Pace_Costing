using CscGet.Audit.Domain.Models;
using CscGet.Costing.Domain.Dispatcher.Events.LaborRates;

namespace CscGet.Audit.Application.Factories
{
    public interface IPyramidMixAuditRecordFactory
    {
        PyramidMixAuditRecord[] CreateAuditRecords(LaborRatePyramidMixValuesChangedEvent @event);
    }
}