namespace Notification.Worker.Services.Interfaces;

public interface IMessageService
{  
    Task SendTextMessageAsync(string phoneNumber, string text);
    Task<string> RegisterDevice(string deviceToken);
    Task DeregisterDevice(string arn);
}
