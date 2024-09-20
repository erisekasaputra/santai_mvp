using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Notification.Worker.Services.Interfaces; 

namespace Notification.Worker.Services;

public class SnsMessageService : IMessageService
{
    private AmazonSimpleNotificationServiceClient _amazonServiceClient;
    private IConfiguration _configuration;
    public SnsMessageService(RegionEndpoint regionEndpoint, IConfiguration configuration)
    { 
        _amazonServiceClient = new AmazonSimpleNotificationServiceClient(regionEndpoint);
        _configuration = configuration;
    }
    public async Task SendTextMessageAsync(string phoneNumber, string message)
    {
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(message))
        {
            return;
        }

        //var awsConfiguration = _configuration.GetSection().Get();

        //var awsAccessKey = config["AWS:AccessKey"];
        //var awsSecretKey = config["AWS:SecretKey"];
        //var awsRegion = config["AWS:Region"]; 

        //// Create AWS SNS client with credentials
        //var credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
        //var snsClient = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpoint.GetBySystemName(awsRegion));

        //var request = new PublishRequest
        //{
        //    Message = message,
        //    PhoneNumber = phoneNumber // Format nomor harus +62 untuk Indonesia atau sesuai negara
        //};

        //try
        //{
        //    var response = await snsClient.PublishAsync(request);
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Failed to send message: " + ex.Message);
        //}
    }
}
