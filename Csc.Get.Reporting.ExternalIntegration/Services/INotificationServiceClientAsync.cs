using System.Threading.Tasks;
using Dxc.Captn.NotificationService.Client.Events.Base;

namespace Csc.Get.Reporting.ExternalIntegration.Services
{
    public interface INotificationServiceClientAsync
    {
        Task PublishAsync(BaseEvent @event);
    }
}