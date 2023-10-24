using System.Threading.Tasks;
using CscGet.MessageContracts.NotificationService;
using Dxc.Captn.Infrastructure.Configuration.LogManager;
using Dxc.Captn.NotificationService.Client.Events.Base;
using MassTransit;

namespace Csc.Get.Reporting.ExternalIntegration.Services
{
    public class NotificationServiceClient : INotificationServiceClientAsync
    {
        private readonly IBus _bus;
        private readonly ICorrelationLogManager _correlationLogManager;

        public NotificationServiceClient(IBus bus, ICorrelationLogManager correlationLogManager)
        {
            _bus = bus;
            _correlationLogManager = correlationLogManager;
        }
        
        public Task PublishAsync(BaseEvent @event)
        {
            return _bus.Publish(new PushNotificationMessage(@event.Id, @event, @event.EventType),
                x =>
                {
                    x.Headers.Set("LogCorrelationId", _correlationLogManager.CorrelationId);
                    x.Headers.Set("JwtToken", _correlationLogManager.JwtToken);
                });
        }
    }
}
