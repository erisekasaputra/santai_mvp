namespace Notification.Worker.Services.Interfaces;

public interface IMessageService
{ 
    Task SendTextMessageAsync(string phoneNumber, string text);
}
