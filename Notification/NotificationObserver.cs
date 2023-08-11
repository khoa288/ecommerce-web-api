using Microsoft.AspNetCore.SignalR;

namespace EcommerceWebApi.Notification
{
    public class NotificationObserver : INotificationObserver
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationObserver(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task UpdateAsync(string userId, string message)
        {
            if (userId == "all")
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            }
            else
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
