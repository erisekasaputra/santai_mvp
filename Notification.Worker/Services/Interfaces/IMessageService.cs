using Amazon.SimpleNotificationService.Model;

namespace Notification.Worker.Services.Interfaces;

public interface IMessageService
{  
    Task PublishSmsAsync(string phoneNumber, string text);
    Task<string> RegisterDevice(string deviceToken);
    Task DeregisterDevice(string arn);
    Task PublishPushNotificationAsync(PublishRequest request);
}
