namespace CscGet.CommandDelivery.Notification
{
    public interface INotificationService
    {
        void SendDictionaryUpdatedNotification(string name, bool isTestSet);
    }
}