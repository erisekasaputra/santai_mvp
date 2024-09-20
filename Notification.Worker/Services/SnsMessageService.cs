using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Notification.Worker.Services.Interfaces;

namespace Notification.Worker.Services;

public class SnsMessageService : IMessageService
{
    private AmazonSimpleNotificationServiceClient _amazonServiceClient;
    public SnsMessageService(RegionEndpoint regionEndpoint)
    { 
        _amazonServiceClient = new AmazonSimpleNotificationServiceClient(regionEndpoint);
    }
    public async Task SendTextMessageAsync(string phoneNumber, string text)
    {
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(text))
        {
            return;
        }

        // Now actually send the message.
        var request = new PublishRequest
        {
            Message = text,
            PhoneNumber = phoneNumber,
        };

        try
        { 
            var response = await _amazonServiceClient.PublishAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex}");
        }
    }
}
