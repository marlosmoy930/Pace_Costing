using System.Linq;
using CscGet.Audit.Domain.Models;
using CscGet.Costing.Domain.Dispatcher.Events.LaborRates;

namespace CscGet.Audit.Application.Factories
{
    public class PyramidMixAuditRecordFactory : IPyramidMixAuditRecordFactory
    {
        public PyramidMixAuditRecord[] CreateAuditRecords(LaborRatePyramidMixValuesChangedEvent @event)
        {
            return @event.PyramidMixChangedElementModels.Select(x => new PyramidMixAuditRecord(x.ElementId, @event.CostingVersionId, @event.EventDate, @event.UserId, @event.Username, CreateValues(x.ValuesBeforeChange), CreateValues(x.PyramidMixValuesAfterChange), AuditExceptionReasons.PyramidMixChanged, @event.CostGroupId)).ToArray();
        }

        private static PyramidMixValue[] CreateValues(PyramidMixValueModel[] pyramidMixValueModels)
        {
            return pyramidMixValueModels.Select(x => new PyramidMixValue(x.YearNumber, x.Value)).ToArray();
        }
    }
}
