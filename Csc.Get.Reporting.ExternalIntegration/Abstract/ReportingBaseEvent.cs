using CscGet.MessageContracts.NotificationService;
using Dxc.Captn.NotificationService.Client.Events.Base;

namespace Csc.Get.Reporting.ExternalIntegration.Abstract
{
    public abstract class ReportingBaseEvent : BaseEvent {
        protected ReportingBaseEvent(string id, NotificationEvent eventType) : base(id, null, eventType)
        {
        }
    }
}
