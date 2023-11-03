using System.Threading.Tasks;
using CscGet.CommandDelivery.Constants;
using CscGet.MessageContracts.NotificationService;
using CscGet.MessageContracts.NotificationService.Enums;
using MassTransit;

namespace CscGet.CommandDelivery.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IBus _bus;

        public NotificationService(IBus bus)
        {
            _bus = bus;
        }
        public void SendDictionaryUpdatedNotification(string name, bool isTestSet)
        {
            Task.Run(async () => await _bus.Publish(new PushNotificationMessage(NotificationConstants.BroadcastMessageBidId.ToString(), new { name, isTestSet },
                NotificationEvent.DictionaryUpdated))).GetAwaiter().GetResult();
        }
    }
}