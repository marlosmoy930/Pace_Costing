using System.Threading.Tasks;
using CscGet.Audit.Application.Services;
using Dxc.Captn.Costing.Contracts.Quantity;
using MassTransit;

namespace CscGet.Audit.Application.Handlers
{
    public class QuantitiesEventHandler : IConsumer<QuantitiesChangedEvent>
    {
        private readonly IQuantityAuditService _quantityAuditService;

        public QuantitiesEventHandler(IQuantityAuditService quantityAuditService)
        {
            _quantityAuditService = quantityAuditService;
        }

        public async Task Consume(ConsumeContext<QuantitiesChangedEvent> context)
        {
            var @event = context.Message;

            await _quantityAuditService.CreateOrUpdateExistingRecord(@event).ConfigureAwait(false);
        }
    }
}
