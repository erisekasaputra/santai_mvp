using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel; 
using Core.Utilities; 
using Notification.Worker.Domain;
using Notification.Worker.Services.Interfaces; 

namespace Notification.Worker.Services;

public class NotificationService : INotificationService
{
    private readonly IDynamoDBContext _dynamoDBContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService( 
       IDynamoDBContext dynamoDBContext,
       ILogger<NotificationService> logger)
    { 
        _dynamoDBContext = dynamoDBContext;
        _logger = logger;
    }

    public async Task<List<Notify>> GetNotifications(string belongsTo, int pageSize = 10, long lastTimestamp = 0L)
    {  
        var queryConfig = new QueryOperationConfig
        {
            IndexName = "BelongsTo-Timestamp-index", 
            KeyExpression = new Expression
            {
                ExpressionStatement = "#belongsTo = :belongsTo and #timestamp > :lastTimestamp",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#belongsTo", nameof(Notify.BelongsTo) }, // Partition Key pada GSI
                    { "#timestamp", nameof(Notify.Timestamp) }  // Sort Key pada GSI
                },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    { ":belongsTo", belongsTo },
                    { ":lastTimestamp", lastTimestamp } 
                }
            },
            BackwardSearch = false, // Urutkan dari yang lebih lama ke terbaru (atau sesuaikan dengan kebutuhan)
            Limit = pageSize // Ambil sejumlah item yang diinginkan per halaman
        };

        var search = _dynamoDBContext.FromQueryAsync<Notify>(queryConfig);
         
        return await search.GetNextSetAsync();
    }

    public async Task<bool> SaveNotification(Notify notification)
    {
        try
        {
            await _dynamoDBContext.SaveAsync(notification);
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Failed to save notification into database dynamo", ex);
        }
    }
}
