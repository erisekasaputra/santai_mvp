using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Chat.API.Applications.Services.Interfaces;
using Core.Configurations;
using Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Chat.API.Applications.Services;

public class DynamoDBChatService : IChatService
{
    private readonly IEncryptionService _encryptionService;
    private readonly IAmazonDynamoDB _dynamoDbClient; 

    private readonly string TableName;
    private readonly string PartitionKey;
    private readonly string SortKey;

    public DynamoDBChatService(
        IAmazonDynamoDB amazonDynamoDB,
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

        TableName = configuration["DynamoDB:Chat:TableName"] ?? throw new Exception("Table Name should be set on initialization");
        PartitionKey = configuration["DynamoDB:Chat:PartitionKey"] ?? throw new Exception("Partition Key should be set on initialization");
        SortKey = configuration["DynamoDB:Chat:SortKey"] ?? throw new Exception("Sort Key should be set on initialization");
    }

    public async Task<string> SaveChatMessageAsync(
        string originUserId,
        string destinationUserId,
        string text,
        string? replyMessageId,
        string? replyMessageText,
        long timestamp)
    {

        if (string.IsNullOrWhiteSpace(originUserId)) throw new ArgumentException("originUserId cannot be null or empty");
        if (string.IsNullOrWhiteSpace(destinationUserId)) throw new ArgumentException("destinationUserId cannot be null or empty");
        if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text cannot be null or empty");

        string messageId = Guid.NewGuid().ToString();
        var item = new Dictionary<string, AttributeValue>
        {
            { PartitionKey, new AttributeValue { S = $"{originUserId}#{destinationUserId}" } },
            { SortKey, new AttributeValue { N = timestamp.ToString() } },
            { "messageId", new AttributeValue { S = messageId } },
            { "encryptedText", new AttributeValue { S = await _encryptionService.EncryptAsync(text) } },
            { "replyMessageId", new AttributeValue { S = string.IsNullOrEmpty(replyMessageId) ? string.Empty : replyMessageId} },
            { "replyMessageEncryptedText", new AttributeValue { S = string.IsNullOrEmpty(replyMessageText) ? string.Empty : await _encryptionService.EncryptAsync(replyMessageText) } }
        };

        var request = new PutItemRequest
        {
            TableName = this.TableName,
            Item = item
        };

        try
        {
            await _dynamoDbClient.PutItemAsync(request); 
        }
        catch (Exception)
        {
            throw;
        }

        return messageId;
    }

    public async Task<List<Dictionary<string, AttributeValue>>> GetMessageByTimestamp(string originUserId, string destinationUserId, long timestamp, bool forward = true)
    {
        if (string.IsNullOrWhiteSpace(originUserId)) throw new ArgumentException("originUserId cannot be null or empty");
        if (string.IsNullOrWhiteSpace(destinationUserId)) throw new ArgumentException("destinationUserId cannot be null or empty");

        // Gabungkan originUserId dan destinationUserId menjadi chatKey
        var chatKey = $"{originUserId}#{destinationUserId}";
        var request = new QueryRequest
        {
            TableName = this.TableName,
            KeyConditionExpression = $"{PartitionKey} = :v_partitionKey AND {SortKey} {(forward ? ">" : "<")} :v_sortKey",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":v_partitionKey", new AttributeValue { S = chatKey } },
                { ":v_sortKey", new AttributeValue { N = timestamp.ToString() } }
            },
            ScanIndexForward = forward,  
            Limit = 200 
        };

        try
        {
            var response = await _dynamoDbClient.QueryAsync(request);
            return response.Items;
        }
        catch (Exception ex)
        {
            // Log atau tangani exception
            throw new Exception("Error retrieving messages", ex);
        }
    }
}
