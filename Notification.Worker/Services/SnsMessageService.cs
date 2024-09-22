using Amazon; 
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core.Configurations;
using Core.Utilities;
using MassTransit.AmazonSqsTransport;
using Microsoft.Extensions.Options;
using Notification.Worker.Services.Interfaces; 

namespace Notification.Worker.Services;

public class SnsMessageService : IMessageService
{
    private readonly AmazonSimpleNotificationServiceClient _snsClient; 
    private readonly AWSIAMConfiguration _awsConfig;
    private readonly ILogger<SnsMessageService> _logger; 
    public SnsMessageService( 
        IOptionsMonitor<AWSIAMConfiguration> awsConfig, 
        ILogger<SnsMessageService> logger)
    {
        _awsConfig = awsConfig.CurrentValue;
        var awsOptions = new AmazonSimpleNotificationServiceConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_awsConfig.Region)
        };

        _snsClient = new AmazonSimpleNotificationServiceClient(
            _awsConfig.AccessID,
            _awsConfig.SecretKey,
            awsOptions);
        _logger = logger; 
    } 

    public async Task RegisterDevice(string deviceToken)
    {
        try
        {
            if (string.IsNullOrEmpty(deviceToken))
            {
                return;
            }

            //var request = new CreatePlatformEndpointRequest
            //{
            //    PlatformApplicationArn = platformApplicationArn,
            //    Token = deviceToken
            //};
        }
        catch (AmazonSimpleNotificationServiceException ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }
    }

    public async Task SendTextMessageAsync(string phoneNumber, string message)
    { 
        try
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(message))
            {
                return;
            }

            var request = new PublishRequest
            {
                Message = message,
                PhoneNumber = phoneNumber,
            };

            var response = await _snsClient.PublishAsync(request);
            response.EnsureSuccessfulResponse();
        }
        catch (AmazonSimpleNotificationServiceException ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
        }
    }
}
