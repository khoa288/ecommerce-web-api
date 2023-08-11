namespace EcommerceWebApi.Notification
{
    public interface INotificationObserver
    {
        Task UpdateAsync(string userId, string message);
    }
}
