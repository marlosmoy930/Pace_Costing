using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Application.Services;
using Dxc.Captn.Costing.Contracts.Sagas.Messages.Audit;
using MassTransit;

namespace CscGet.Audit.Application.Handlers
{
    public class BidEventHandler : IConsumer<IAuditCopyDataCommand>
    {
        private readonly IQuantityAuditService _quantityAuditService;
        private readonly IGlobalTotalEntityAuditService _globalTotalEntityAuditService;
        private readonly IPyramidMixAuditService _pyramidMixAuditService;

        public BidEventHandler(IQuantityAuditService quantityAuditService, IGlobalTotalEntityAuditService globalTotalEntityAuditService, IPyramidMixAuditService pyramidMixAuditService)
        {
            _quantityAuditService = quantityAuditService;
            _globalTotalEntityAuditService = globalTotalEntityAuditService;
            _pyramidMixAuditService = pyramidMixAuditService;
        }

        public async Task Consume(ConsumeContext<IAuditCopyDataCommand> context)
        {
            var command = context.Message;
            var entityModels = context.Message.CopiedEntities.Select(x => new EntityRecordCopyModel(x.SourceCostGroupId, x.CopiedCostGroupId)).ToArray();
            var elementRecordModels = command.CopiedElements.Select(x => new ElementRecordCopyModel(x.OriginalElementId, x.Element.Id, x.Element.CostGroup.Id)).ToArray();

            Task copyQuantitiesLog = _quantityAuditService.CopyRecordsAsync(command.TargetCostingVersionId, entityModels);
            Task copyGlobalTotalEntitiesLog = _globalTotalEntityAuditService.CopyRecordsAsync(command.TargetCostingVersionId, entityModels);
            Task copyPyramidMixAudit = _pyramidMixAuditService.CopyRecordsAsync(command.TargetCostingVersionId, elementRecordModels);

            await Task.WhenAll(copyQuantitiesLog, copyGlobalTotalEntitiesLog, copyPyramidMixAudit);

            await context.Publish<IAuditCopyDataCommandResponse>(new AuditCopyDataCommandResponse {CorrelationId = command.CorrelationId});
        }
    }
}