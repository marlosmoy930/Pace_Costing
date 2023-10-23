using CscGet.MessageContracts.NotificationService;
using Dxc.Captn.NotificationService.Client.Events.Base;

namespace Csc.Get.Reporting.ExternalIntegration.Models
{
    public class ReportBaseEvent : BaseEvent
    {
        public ReportBaseEvent(string id, ReportNotificationEvent eventType)
            : base(id, null, (NotificationEvent)eventType)
        {
        }
    }
}
