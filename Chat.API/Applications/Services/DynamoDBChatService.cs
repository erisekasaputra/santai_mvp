using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Chat.API.Applications.Services.Interfaces;
using Core.Services.Interfaces;

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
        IConfiguration configuration)
    {
        _dynamoDbClient = amazonDynamoDB;
        _encryptionService = encryptionService;

        TableName = configuration["DynamoDB:Chat:TableName"] ?? throw new Exception("Table Name should be set on initialization");
        PartitionKey = configuration["DynamoDB:Chat:PartitionKey"] ?? throw new Exception("Partition Key should be set on initialization");
        SortKey = configuration["DynamoDB:Chat:SortKey"] ?? throw new Exception("Sort Key should be set on initialization");
    }

    public async Task<string> SaveChatMessageAsync(string originUserId, string destinationUserId, string text, long timestamp)
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
            { "encryptedText", new AttributeValue { S = await _encryptionService.EncryptAsync(text) } }
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
        catch (Exception ex)
        {
            // Log error dan tangani exception dengan baik
            throw new Exception("Failed to save chat message.", ex);
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
            ScanIndexForward = forward,  // Urutkan dari yang tertua ke yang terbaru
            Limit = 200               // Batasi jumlah pesan yang diambil
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
