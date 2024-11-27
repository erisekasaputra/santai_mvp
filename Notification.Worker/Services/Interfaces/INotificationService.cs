using Notification.Worker.Domain;

namespace Notification.Worker.Services.Interfaces;

public interface INotificationService
{
    Task<bool> SaveNotification(Notify notification);
    Task<List<Notify>> GetNotifications(string belongsTo, int pageSize = 10, long lastTimestamp = 0L);
}
