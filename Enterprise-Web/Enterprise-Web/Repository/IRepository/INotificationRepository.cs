using Enterprise_Web.ViewModels;

namespace Enterprise_Web.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task CheckAndSend(NotificationViewModel notificationViewModel);
    }
}
