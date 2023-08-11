namespace EcommerceWebApi.Notification
{
    public class NotificationSubject
    {
        private readonly List<INotificationObserver> _observers = new();

        public void Attach(INotificationObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(INotificationObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyAsync(string userId, string message)
        {
            foreach (var observer in _observers)
            {
                await observer.UpdateAsync(userId, message);
            }
        }
    }
}
