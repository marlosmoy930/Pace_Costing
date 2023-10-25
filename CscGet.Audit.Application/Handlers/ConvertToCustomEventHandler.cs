using System.Threading.Tasks;
using CscGet.Audit.Application.Services;
using CscGet.Costing.Domain.Dispatcher.Handlers;
using Dxc.Captn.Costing.Contracts.Operations.ConvertToCustom;

namespace CscGet.Audit.Application.Handlers
{
    public class ConvertToCustomEventHandler : IContextEventHandlerAsync<CreateConversionAuditRecordCommand>
    {
        private readonly IConvertToCustomAuditService _convertToCustomAuditService;
        private readonly IUserProvider _userProvider;

        public ConvertToCustomEventHandler(IConvertToCustomAuditService convertToCustomAuditService, IUserProvider userProvider)
        {
            _convertToCustomAuditService = convertToCustomAuditService;
            _userProvider = userProvider;
        }

        public async Task Handle(CreateConversionAuditRecordCommand @event)
        {
            var userName = _userProvider.GetCurrentUserName();
            await _convertToCustomAuditService.CreateRecord(@event, userName).ConfigureAwait(false);
        }
    }
}
