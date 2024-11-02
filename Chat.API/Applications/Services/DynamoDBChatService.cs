using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Models;
using Core.Configurations;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Chat.API.Applications.Services;

public class DynamoDBChatService : IChatService
{
    private readonly IEncryptionService _encryptionService;
    private readonly AmazonDynamoDBClient _dynamoDbClient; 

    private readonly string ConversationTableName;
    private readonly string ConversationPartitionKey;
    private readonly string ConversationSortKey;

    private readonly string ChatContactTableName;
    private readonly string ChatContactPartitionKey;
    private readonly string ChatContactSortKey;

    public DynamoDBChatService( 
        IEncryptionService encryptionService,
        IConfiguration configuration,
        IOptionsMonitor<AWSIAMConfiguration> awsIamConfiguration)
    {
        var iam = awsIamConfiguration.CurrentValue;
        // Create AWS credentials
        var credentials = new BasicAWSCredentials(iam.AccessID, iam.SecretKey);

        // Initialize DynamoDB client with credentials and region
        var regionEndpoint = RegionEndpoint.GetBySystemName(iam.Region);
        _dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint);
        _encryptionService = encryptionService;


        ConversationTableName = configuration["DynamoDB:Chat:ConversationTableName"] ?? throw new Exception("ConversationTableName name should be set on initialization");
        ConversationPartitionKey = configuration["DynamoDB:Chat:ConversationPartitionKey"] ?? throw new Exception("ConversationPartitionKey key should be set on initialization");
        ConversationSortKey = configuration["DynamoDB:Chat:ConversationSortKey"] ?? throw new Exception("ConversationSortKey key should be set on initialization");


        ChatContactTableName = configuration["DynamoDB:Chat:ChatContactTableName"] ?? throw new Exception("ChatContactTableName name should be set on initialization");
        ChatContactPartitionKey = configuration["DynamoDB:Chat:ChatContactPartitionKey"] ?? throw new Exception("ChatContactPartitionKey key should be set on initialization");
        ChatContactSortKey = configuration["DynamoDB:Chat:ChatContactSortKey"] ?? throw new Exception("ChatContactSortKey key should be set on initialization");
    }

    public async Task<bool> SaveChatMessageAsync(Conversation conversation)
    { 
        string encryptedText;
        try
        {
            encryptedText = string.IsNullOrEmpty(conversation.Text) ? string.Empty : await _encryptionService.EncryptAsync(conversation.Text);
        }
        catch (Exception)
        {
            encryptedText = string.Empty; 
        }

        string encryptedReplyMessageText;
        try
        {
            encryptedReplyMessageText = string.IsNullOrEmpty(conversation.ReplyMessageText) ? string.Empty : await _encryptionService.EncryptAsync(conversation.ReplyMessageText);
        }
        catch (Exception)
        {
            encryptedReplyMessageText = string.Empty; 
        }

        var item = new Dictionary<string, AttributeValue>
        {
            { ConversationPartitionKey, new AttributeValue { S = conversation.MessageId.ToString() } },
            { ConversationSortKey, new AttributeValue { N = conversation.TimeStamp.ToString() } },
            { nameof(conversation.OrderId), new AttributeValue { S = conversation.OrderId.ToString() } },
            { nameof(conversation.OriginUserId), new AttributeValue { S = conversation.OriginUserId.ToString() } },
            { nameof(conversation.DestinationUserId), new AttributeValue { S = conversation.DestinationUserId.ToString() } },
            { nameof(conversation.Text), new AttributeValue { S = encryptedText } },
            { nameof(conversation.Attachment), new AttributeValue { S = string.IsNullOrEmpty(conversation.Attachment) ? string.Empty : conversation.Attachment } },
            { nameof(conversation.ReplyMessageId), new AttributeValue { S = conversation.ReplyMessageId is null ? string.Empty : conversation.ReplyMessageId.ToString() } },
            { nameof(conversation.ReplyMessageText), new AttributeValue { S = encryptedReplyMessageText } }
        };

        var request = new PutItemRequest
        {
            TableName = ConversationTableName,
            Item = item
        };

        try
        {
            await _dynamoDbClient.PutItemAsync(request);
            return true;
        }
        catch (Exception ex)
        { 
            throw new Exception("Failed to put item into DynamoDB", ex);
        } 
    }

    public async Task<List<Conversation>?> GetMessageByTimestamp(Guid orderId, long timestamp, bool forward = true)
    {
        var request = new QueryRequest
        {
            TableName = ConversationTableName,
            IndexName = "OrderId-TimeStamp-index", 
            KeyConditionExpression = $"{nameof(Conversation.OrderId)} = :v_orderId AND {nameof(Conversation.TimeStamp)} {(forward ? ">" : "<")} :v_sortKey",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_orderId", new AttributeValue { S = orderId.ToString() } },
                { ":v_sortKey", new AttributeValue { N = timestamp.ToString() } }
            },
            ScanIndexForward = forward,
            Limit = 100
        };

        try
        {
            var response = await _dynamoDbClient.QueryAsync(request);

            var tasks = response.Items.Select(async message =>
            {
                var stringMessageId = message.GetValueOrDefault(nameof(Conversation.MessageId))?.S;
                var stringOrderId = message.GetValueOrDefault(nameof(Conversation.OrderId))?.S;
                var stringOriginUserId = message.GetValueOrDefault(nameof(Conversation.OriginUserId))?.S;
                var stringDestinationUserId = message.GetValueOrDefault(nameof(Conversation.DestinationUserId))?.S;
                var stringText = message.GetValueOrDefault(nameof(Conversation.Text))?.S;
                var stringAttachment = message.GetValueOrDefault(nameof(Conversation.Attachment))?.S;
                var stringReplyMessageId = message.GetValueOrDefault(nameof(Conversation.ReplyMessageId))?.S;
                var stringReplyMessageText = message.GetValueOrDefault(nameof(Conversation.ReplyMessageText))?.S;
                var stringTimeStamp = message.GetValueOrDefault(nameof(Conversation.TimeStamp))?.N;

                var messageId = string.IsNullOrEmpty(stringMessageId) ? Guid.Empty : Guid.Parse(stringMessageId);
                var retrievedOrderId = string.IsNullOrEmpty(stringOrderId) ? Guid.Empty : Guid.Parse(stringOrderId);
                var originUserId = string.IsNullOrEmpty(stringOriginUserId) ? Guid.Empty : Guid.Parse(stringOriginUserId);
                var destinationUserId = string.IsNullOrEmpty(stringDestinationUserId) ? Guid.Empty : Guid.Parse(stringDestinationUserId);

                string text;
                try
                {
                    text = string.IsNullOrEmpty(stringText) ? string.Empty : await _encryptionService.DecryptAsync(stringText);
                }
                catch (Exception)
                {
                    text = string.Empty;
                }

                var attachment = stringAttachment;
                var replyMessageId = string.IsNullOrEmpty(stringReplyMessageId) ? Guid.Empty : Guid.Parse(stringReplyMessageId);
                var replyMessageText = stringReplyMessageText;

                if (!long.TryParse(stringTimeStamp, out long timeStamp))
                {
                    timeStamp = 0;
                }

                return new Conversation(
                    messageId,
                    retrievedOrderId,
                    originUserId,
                    destinationUserId,
                    text,
                    attachment,
                    replyMessageId,
                    replyMessageText,
                    timeStamp);
            });

            var results = await Task.WhenAll(tasks);
            return [..results];
        }
        catch (Exception ex)
        { 
            throw new Exception("Error retrieving messages", ex);
        }
    }



    public async Task<bool> CreateOrderContact(ChatContact chatContact)
    { 
        string encryptedText;
        try
        {
            encryptedText = string.IsNullOrEmpty(chatContact.LastChatText)
                            ? string.Empty
                            : await _encryptionService.EncryptAsync(chatContact.LastChatText);
        }
        catch (Exception)
        {
            encryptedText = string.Empty; 
        }
         
        var item = new Dictionary<string, AttributeValue>
        {
            { ChatContactPartitionKey, new AttributeValue { S = chatContact.OrderId.ToString() } },
            { ChatContactSortKey, new AttributeValue { N = chatContact.LastChatTimestamp.ToString() } },
            { nameof(chatContact.OrderId), new AttributeValue { S = chatContact.OrderId.ToString() } },
            { nameof(chatContact.BuyerId), new AttributeValue { S = chatContact.BuyerId.ToString() } },
            { nameof(chatContact.BuyerName), new AttributeValue { S = chatContact.BuyerName } },
            { nameof(chatContact.MechanicId), new AttributeValue { S = chatContact.MechanicId?.ToString() ?? string.Empty } },
            { nameof(chatContact.MechanicName), new AttributeValue { S = chatContact.MechanicName ?? string.Empty } },
            { nameof(chatContact.LastChatText), new AttributeValue { S = encryptedText } },
            { nameof(chatContact.LastChatTimestamp), new AttributeValue { N = chatContact.LastChatTimestamp.ToString() } },
            { nameof(chatContact.OrderCompletedAtUtc), new AttributeValue { S = chatContact.OrderCompletedAtUtc?.ToString("o") ?? string.Empty } }, 
            { nameof(chatContact.OrderChatExpiredAtUtc), new AttributeValue { S = chatContact.OrderChatExpiredAtUtc?.ToString("o") ?? string.Empty } },
            { nameof(chatContact.IsOrderCompleted), new AttributeValue { BOOL = chatContact.IsOrderCompleted } }
        };

        var request = new PutItemRequest
        {
            TableName = ChatContactTableName,
            Item = item
        };

        try
        {
            await _dynamoDbClient.PutItemAsync(request);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to put item into DynamoDB", ex);
        }
    }


    public async Task<List<ChatContact>?> GetMessagesByBuyerId(Guid buyerId, long timestamp, bool forward = true)
    {
        var request = new QueryRequest
        {
            TableName = ChatContactTableName,
            IndexName = "BuyerId-LastChatTimestamp-index", // GSI for BuyerId
            KeyConditionExpression = $"{nameof(ChatContact.BuyerId)} = :v_buyerId AND {nameof(ChatContact.LastChatTimestamp)} {(forward ? " > " : " < ")} :v_sortKey",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_buyerId", new AttributeValue { S = buyerId.ToString() } },
                { ":v_sortKey", new AttributeValue { N = timestamp.ToString() } }
            },
            ScanIndexForward = forward,
            Limit = 100
        };

        try
        {
            var response = await _dynamoDbClient.QueryAsync(request);

            var tasks = response.Items.Select(async item =>
            {
                return await MapChatContactAsync(item);
            });

            var results = await Task.WhenAll(tasks);
            return [..results];
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving messages by BuyerId", ex);
        }
    }

    public async Task<List<ChatContact>?> GetMessagesByMechanicId(Guid mechanicId, long timestamp, bool forward = true)
    {
        var request = new QueryRequest
        {
            TableName = ChatContactTableName,
            IndexName = "Mechanicid-LastChatTimestamp-index", // GSI for MechanicId
            KeyConditionExpression = $"{nameof(ChatContact.MechanicId)} = :v_mechanicId AND {nameof(ChatContact.LastChatTimestamp)} {(forward ? ">" : "<")} :v_sortKey",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_mechanicId", new AttributeValue { S = mechanicId.ToString() } },
                { ":v_sortKey", new AttributeValue { N = timestamp.ToString() } }
            },
            ScanIndexForward = forward,
            Limit = 100
        };

        try
        {
            var response = await _dynamoDbClient.QueryAsync(request);

            var tasks = response.Items.Select(async item =>
            {
                return await MapChatContactAsync(item);
            });

            var results = await Task.WhenAll(tasks);
            return [.. results];
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving messages by MechanicId", ex);
        }
    }

    private async Task<ChatContact> MapChatContactAsync(Dictionary<string, AttributeValue> item)
    {
        var stringOrderId = item.GetValueOrDefault(nameof(ChatContact.OrderId))?.S;
        var stringBuyerId = item.GetValueOrDefault(nameof(ChatContact.BuyerId))?.S;
        var buyerName = item.GetValueOrDefault(nameof(ChatContact.BuyerName))?.S;
        var stringMechanicId = item.GetValueOrDefault(nameof(ChatContact.MechanicId))?.S;
        var mechanicName = item.GetValueOrDefault(nameof(ChatContact.MechanicName))?.S;
        var stringLastChatText = item.GetValueOrDefault(nameof(ChatContact.LastChatText))?.S;
        var stringTimeStamp = item.GetValueOrDefault(nameof(ChatContact.LastChatTimestamp))?.N;
        var stringOrderCompletedAtUtc = item.GetValueOrDefault(nameof(ChatContact.OrderCompletedAtUtc))?.S;
        var stringOrderChatExpiredAtUtc = item.GetValueOrDefault(nameof(ChatContact.OrderChatExpiredAtUtc))?.S;
        var isOrderCompleted = item.GetValueOrDefault(nameof(ChatContact.IsOrderCompleted))?.BOOL ?? false;

        var orderId = string.IsNullOrEmpty(stringOrderId) ? Guid.Empty : Guid.Parse(stringOrderId);
        var buyerId = string.IsNullOrEmpty(stringBuyerId) ? Guid.Empty : Guid.Parse(stringBuyerId);
        var mechanicId = string.IsNullOrEmpty(stringMechanicId) ? (Guid?)null : Guid.Parse(stringMechanicId);

        string lastChatText;
        try
        {
            lastChatText = string.IsNullOrEmpty(stringLastChatText) ? string.Empty : await _encryptionService.DecryptAsync(stringLastChatText);
        }
        catch (Exception)
        {
            lastChatText = string.Empty;
        }

        if (!long.TryParse(stringTimeStamp, out long timestamp))
        {
            timestamp = 0;
        }

        DateTime? orderCompletedAtUtc = string.IsNullOrEmpty(stringOrderCompletedAtUtc) ? (DateTime?)null : DateTime.Parse(stringOrderCompletedAtUtc);
        DateTime? orderChatExpiredAtUtc = string.IsNullOrEmpty(stringOrderChatExpiredAtUtc) ? (DateTime?)null : DateTime.Parse(stringOrderChatExpiredAtUtc);

        return new ChatContact(orderId, buyerId, buyerName ?? string.Empty, timestamp)
        {
            MechanicId = mechanicId,
            MechanicName = mechanicName,
            LastChatText = lastChatText,
            LastChatTimestamp = timestamp,
            OrderCompletedAtUtc = orderCompletedAtUtc,
            OrderChatExpiredAtUtc = orderChatExpiredAtUtc,
            IsOrderCompleted = isOrderCompleted
        };
    }

    public async Task<ChatContact?> GetChatContactByOrderId(Guid orderId)
    {
        var request = new GetItemRequest
        {
            TableName = ChatContactTableName,
            Key = new Dictionary<string, AttributeValue>
            {
                { nameof(ChatContact.OrderId), new AttributeValue { S = orderId.ToString() } }
            }
        };

        try
        {
            var response = await _dynamoDbClient.GetItemAsync(request);

            if (response.Item == null || response.Item.Count == 0)
            {
                return null;  
            }
             
            return await MapChatContactAsync(response.Item);
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving ChatContact by OrderId", ex);
        }
    }
}
