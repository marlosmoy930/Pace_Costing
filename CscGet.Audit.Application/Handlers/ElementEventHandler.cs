using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Application.Services;
using CscGet.Audit.Application.Services.External;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Costing.Domain.Dispatcher.Events.Element;
using CscGet.Costing.Domain.Dispatcher.Events.Element.Copy;
using CscGet.Costing.Domain.Dispatcher.Events.LaborRates;
using CscGet.Costing.Domain.Dispatcher.Handlers;

namespace CscGet.Audit.Application.Handlers
{
    public class ElementEventHandler :
        IContextEventHandlerAsync<ElementsDeleted>,
        IContextEventHandlerAsync<LaborRatePyramidMixValuesChangedEvent>,
        IContextEventHandlerAsync<ElementsCopied>
    {
        private readonly IPyramidMixAuditService _pyramidMixAuditService;
        private readonly IDataSourceTypeService _dataSourceTypeService;

        public ElementEventHandler(IPyramidMixAuditService pyramidMixAuditService, IDataSourceTypeService dataSourceTypeService)
        {
            _pyramidMixAuditService = pyramidMixAuditService;
            _dataSourceTypeService = dataSourceTypeService;
        }

        public async Task Handle(ElementsDeleted @event)
        {
            var laborElementIds = @event.Elements.Where(x => x.CostGroup.GroupTypeCode == GroupType.LRT.ToString()).Select(x => x.Id).ToArray();
            if (!laborElementIds.Any())
                return;

           await _pyramidMixAuditService.RemoveRecordsAsync(laborElementIds).ConfigureAwait(false);
        }

        public async Task Handle(LaborRatePyramidMixValuesChangedEvent @event)
        {
            var eligibleIds = await _dataSourceTypeService.FilterOutEntityIdsWithSctDataSource(
                @event.CostingVersionId, new[] {@event.CostGroupId});

            if (!eligibleIds.Any())
                return;

            await _pyramidMixAuditService.CreateOrUpdateRecordsAsync(@event).ConfigureAwait(false);
        }

        public async Task Handle(ElementsCopied @event)
        {
            var elementRecordModels = @event.Elements.Select(x => new ElementRecordCopyModel(x.OriginalElementId, x.Element.Id, x.Element.CostGroup.Id)).ToArray();
            await _pyramidMixAuditService.CopyRecordsAsync(@event.TargetBidId, elementRecordModels).ConfigureAwait(false);
        }
    }
}
