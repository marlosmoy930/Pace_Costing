using System.Threading.Tasks;
using Csc.Get.Reporting.ExternalIntegration.Abstract;
using Csc.Get.Reporting.ExternalIntegration.Models;

namespace Csc.Get.Reporting.ExternalIntegration.Services
{
    public class ExternalNotificationService : IExternalNotificationService
    {
        private readonly INotificationServiceClientAsync _notificationServiceClientAsync;

        public ExternalNotificationService(INotificationServiceClientAsync notificationServiceClientAsync)
        {
            _notificationServiceClientAsync = notificationServiceClientAsync;
        }
        
        public Task PublishAsync(ReportBaseEvent @event)
        {
            return _notificationServiceClientAsync.PublishAsync(@event);
        }
    }
}
