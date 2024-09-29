using Amazon; 
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core.Configurations;
using Core.Utilities; 
using Microsoft.Extensions.Options; 
using Notification.Worker.Services.Interfaces;
using System.Net;

namespace Notification.Worker.Services;

public class SnsMessageService : IMessageService
{
    private readonly AmazonSimpleNotificationServiceClient _snsClient; 
    private readonly AWSIAMConfiguration _awsConfig;
    private readonly AWSSNSConfiguration _snsConfig;
    private readonly ILogger<SnsMessageService> _logger; 
    public SnsMessageService( 
        IOptionsMonitor<AWSIAMConfiguration> awsConfig, 
        ILogger<SnsMessageService> logger,
        IOptionsMonitor<AWSSNSConfiguration> snsConfig)
    {   
        _awsConfig = awsConfig.CurrentValue;
        _snsConfig = snsConfig.CurrentValue;
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

    public async Task DeregisterDevice(string arn)
    {
        try
        {
            if (string.IsNullOrEmpty(arn))
            {
                return;
            }

            var request = new DeleteEndpointRequest
            { 
                EndpointArn = arn
            };

            await _snsClient.DeleteEndpointAsync(request);   
            return;
        }
        catch (AmazonSimpleNotificationServiceException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;  
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }

    public async Task PublishPushNotificationAsync(PublishRequest request)
    {
        try
        { 
            await _snsClient.PublishAsync(request);
        }
        catch (AmazonSimpleNotificationServiceException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }
     

    public async Task<string> RegisterDevice(string deviceToken)
    {
        try
        {
            if (string.IsNullOrEmpty(deviceToken))
            {
                return string.Empty;
            }

            var request = new CreatePlatformEndpointRequest
            {
                PlatformApplicationArn = _snsConfig.ARN,
                Token = deviceToken
            };

            var response = await _snsClient.CreatePlatformEndpointAsync(request); 
            if (!string.IsNullOrEmpty(response.EndpointArn)) 
            {
                return response.EndpointArn;
            }

            return string.Empty;
        }
        catch (AmazonSimpleNotificationServiceException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }

    public async Task PublishSmsAsync(string phoneNumber, string message)
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
                PhoneNumber = phoneNumber
            };

            var response = await _snsClient.PublishAsync(request); 
        }
        catch (AmazonSimpleNotificationServiceException ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw;
        }
    }
}
