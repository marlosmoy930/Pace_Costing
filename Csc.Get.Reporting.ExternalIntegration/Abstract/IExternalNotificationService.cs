using System.Threading.Tasks;
using Csc.Get.Reporting.ExternalIntegration.Models;

namespace Csc.Get.Reporting.ExternalIntegration.Abstract
{
    public interface IExternalNotificationService
    {
        Task PublishAsync(ReportBaseEvent @event);
    }
}
