using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Application.Services;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Costing.Domain.CostGroups.Entities;
using CscGet.Costing.Domain.Dispatcher.Events.CostGroup;
using CscGet.Costing.Domain.Dispatcher.Events.Wbs.CostGroups;
using CscGet.Costing.Domain.Dispatcher.Handlers;

namespace CscGet.Audit.Application.Handlers
{
    public class CostGroupEventHandler : IContextEventHandlerAsync<CostGroupsCopied>, IContextEventHandlerAsync<CostGroupsDeleted>
    {
        private readonly IQuantityAuditService _quantityAuditService;
        private readonly IPyramidMixAuditService _pyramidMixAuditService;
        private readonly IGlobalTotalEntityAuditService _globalTotalEntityAuditService;

        public CostGroupEventHandler(IQuantityAuditService quantityAuditService, IPyramidMixAuditService pyramidMixAuditService, IGlobalTotalEntityAuditService globalTotalEntityAuditService)
        {
            _quantityAuditService = quantityAuditService;
            _pyramidMixAuditService = pyramidMixAuditService;
            _globalTotalEntityAuditService = globalTotalEntityAuditService;
        }

        public async Task Handle(CostGroupsDeleted @event)
        {
            var costGroupIds = @event.CostGroups.Select(x => x.Id).ToArray();
            await Task.WhenAll(_quantityAuditService.RemoveRecordsAsync(costGroupIds), RemovePyramidMixAuditRecordsForCostGroups(@event.CostGroups), _globalTotalEntityAuditService.RemoveRecordsAsync(costGroupIds)).ConfigureAwait(false);
        }

        public async Task Handle(CostGroupsCopied @event)
        {
            var models = @event.CostGroupCopiedModels.Select(x => new EntityRecordCopyModel(x.SourceCostGroupId, x.CopiedCostGroupId)).ToArray();
            Task copyQuantitiesLog = _quantityAuditService.CopyRecordsAsync(@event.TargetBidId, models);
            Task copyGlobalTotalEntitiesLog = _globalTotalEntityAuditService.CopyRecordsAsync(@event.TargetBidId, models);

            await Task.WhenAll(copyQuantitiesLog, copyGlobalTotalEntitiesLog).ConfigureAwait(false);
        }

        private Task RemovePyramidMixAuditRecordsForCostGroups(List<CostGroup> costGroups)
        {
            var laborRateGroupIds = costGroups.Where(x => x.Type.Code == GroupType.LRT.ToString()).Select(x => x.Id).ToArray();
            if (!laborRateGroupIds.Any())
                return Task.CompletedTask;

            return _pyramidMixAuditService.RemoveRecordsForCostGroupsAsync(laborRateGroupIds);
        }
    }
}
