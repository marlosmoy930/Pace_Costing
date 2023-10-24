using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Application.Services;
using CscGet.Costing.Domain.Dispatcher.Events.BaselineMetric;
using CscGet.Costing.Domain.Dispatcher.Events.Wbs.BaselineMetrics;
using CscGet.Costing.Domain.Dispatcher.Handlers;

namespace CscGet.Audit.Application.Handlers
{
    public class BaselineMetricEventHandler : IContextEventHandlerAsync<BaselineMetricDeleted>, IContextEventHandlerAsync<BaselineMetricsCopied>
    {
        private readonly IQuantityAuditService _quantityAuditService;
        private readonly IGlobalTotalEntityAuditService _globalTotalEntityAuditService;

        public BaselineMetricEventHandler(IQuantityAuditService quantityAuditService, IGlobalTotalEntityAuditService globalTotalEntityAuditService)
        {
            _quantityAuditService = quantityAuditService;
            _globalTotalEntityAuditService = globalTotalEntityAuditService;
        }

        public async Task Handle(BaselineMetricDeleted @event)
        {
            var ids = @event.Models.Select(x => x.BaselineMetricId).ToArray();
            var removalOperations = new[]
            {
                _quantityAuditService.RemoveRecordsAsync(ids),
                _globalTotalEntityAuditService.RemoveRecordsAsync(ids)
            };

            await Task.WhenAll(removalOperations);
        }

        public async Task Handle(BaselineMetricsCopied @event)
        {
            var models = @event.BaselineMetricCopyModels.Select(x => new EntityRecordCopyModel(x.ExistingBaselineMetricId, x.CopiedBaselineMetricId)).ToArray();

            var copyQuantitiesLog = _quantityAuditService.CopyRecordsAsync(@event.TargetBidId, models);
            var copyGlobalTotalEntitiesLog = _globalTotalEntityAuditService.CopyRecordsAsync(@event.TargetBidId, models);

            await Task.WhenAll(copyQuantitiesLog, copyGlobalTotalEntitiesLog).ConfigureAwait(false);
        }
    }
}
