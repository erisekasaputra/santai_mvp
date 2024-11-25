using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;  
using Chat.API.Applications.Services.Interfaces;
using Chat.API.Domain.Models;
using Core.Configurations; 
using Core.Utilities;
using Microsoft.Extensions.Options;

namespace Chat.API.Applications.Services;

public class DynamoDBChatService : IChatService
{ 
    //private readonly AmazonDynamoDBClient _dynamoDbClient;
    private readonly IDynamoDBContext _dynamoDBContext; 
    private readonly ILogger<DynamoDBChatService> _logger;

    public DynamoDBChatService(  
        IConfiguration configuration,
        IOptionsMonitor<AWSIAMConfiguration> awsIamConfiguration,
        IDynamoDBContext dynamoDBContext,
        ILogger<DynamoDBChatService> logger)
    {
        //var iam = awsIamConfiguration.CurrentValue; 
        //var credentials = new BasicAWSCredentials(iam.AccessID, iam.SecretKey); 
        //var regionEndpoint = RegionEndpoint.GetBySystemName(iam.Region);
        //_dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint); 
        _dynamoDBContext = dynamoDBContext;
        _logger = logger;
    }

    public async Task<bool> SaveChatMessageAsync(Conversation conversation)
    {   
        try
        { 
            await _dynamoDBContext.SaveAsync(conversation); 
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Failed to save conversation into DynamoDB", ex);
        } 
    }

    public async IAsyncEnumerable<Conversation> GetMessageByOrderId(string orderId, bool forward = true)
    {
        var queryConfig = new QueryOperationConfig
        {
            IndexName = "OrderId-Timestamp-index",
            KeyExpression = new Expression
            {
                ExpressionStatement = "#orderId = :orderId",
                ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        { "#orderId", nameof(Conversation.OrderId) }
                    },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":orderId", orderId.ToString() }
                    }
            },
            BackwardSearch = !forward,
            Limit = 1
        };

        var search = _dynamoDBContext.FromQueryAsync<Conversation>(queryConfig);

        do
        {
            var items = await search.GetNextSetAsync();
            foreach (var item in items)
            {
                yield return item;
            }

        } while (!string.IsNullOrEmpty(search.PaginationToken));
    } 
     
    public async Task<bool> CreateChatContact(ChatContact chatContact)
    { 
        try
        { 
            await _dynamoDBContext.SaveAsync(chatContact);
            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Failed to save ChatContact into DynamoDB", ex);
        } 
    }


    public async Task<List<ChatContact>?> GetChatContactsByBuyerId(string buyerId)
    {
        var queryConfig = new QueryOperationConfig
        {
            IndexName = "BuyerId-LastChatTimestamp-index",
            KeyExpression = new Expression
            {
                ExpressionStatement = $"#buyerId = :buyerId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#buyerId", nameof(ChatContact.BuyerId) }, 
                },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    { ":buyerId", buyerId.ToString() } 
                }
            },
            BackwardSearch = true,
            Limit = 1000
        };

        try
        {
            var search = _dynamoDBContext.FromQueryAsync<ChatContact>(queryConfig);
            var results = await search.GetNextSetAsync();
             
            var decryptedResults = new List<ChatContact>();
            foreach (var chatContact in results)
            {  
                chatContact.IsExpired(); // this is important, dont remove
                decryptedResults.Add(chatContact);
            }

            return decryptedResults;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Error retrieving messages by BuyerId", ex);
        } 
    }

    public async Task<List<ChatContact>?> GetChatContactsByMechanicId(string mechanicId)
    {
        var queryConfig = new QueryOperationConfig
        {
            IndexName = "MechanicId-LastChatTimestamp-index",
            KeyExpression = new Expression
            {
                ExpressionStatement = $"#mechanicId = :mechanicId",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#mechanicId", nameof(ChatContact.MechanicId) }
                },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    { ":mechanicId", mechanicId.ToString() } 
                }
            },
            BackwardSearch = true,
            Limit = 1000
        };

        try
        {
            var search = _dynamoDBContext.FromQueryAsync<ChatContact>(queryConfig);
            var results = await search.GetNextSetAsync();
             
            var decryptedResults = new List<ChatContact>();
            foreach (var chatContact in results)
            { 
                chatContact.IsExpired(); // dont remove , this is important
                decryptedResults.Add(chatContact);
            }

            return decryptedResults;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Error retrieving messages by MechanicId", ex);
        } 
    } 

    public async Task<ChatContact?> GetChatContactByOrderId(string orderId)
    {
        try
        { 
            var queryResult = await _dynamoDBContext
                .QueryAsync<ChatContact>(orderId)
                .GetRemainingAsync();
             
            var chatContact = queryResult.FirstOrDefault(); 
            chatContact?.IsExpired(); 
            return chatContact;
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Error retrieving ChatContact by OrderId", ex);
        }
    }

    public async Task<bool> UpdateChatContact(ChatContact chatContact)
    {
        try
        {   
            chatContact.IsExpired(); // dont remove this line
            await _dynamoDBContext.SaveAsync(chatContact);  

            return true;
        }
        catch(Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Error updating ChatContact", ex);
        }
    }

    public async Task DeleteChatContact(string orderId)
    {
        try
        {
            var itemsDeleted = 0;
            var itemsPerBatch = 100;

            var scanConditions = new List<ScanCondition>
        {
            new(nameof(Conversation.OrderId), ScanOperator.Equal, orderId.ToString())
        };

            var search = _dynamoDBContext.ScanAsync<Conversation>(scanConditions);

            do
            {
                var itemsToDelete = await search.GetNextSetAsync();

                foreach (var batch in itemsToDelete.Chunk(itemsPerBatch))
                {
                    var batchWrite = _dynamoDBContext.CreateBatchWrite<Conversation>();

                    foreach (var item in batch)
                    {
                        batchWrite.AddDeleteItem(item);
                    }

                    await batchWrite.ExecuteAsync();
                    itemsDeleted += batch.Length;
                }

            } while (!string.IsNullOrEmpty(search.PaginationToken));

            var chatContact = await GetChatContactByOrderId(orderId);
            if (chatContact == null)
            {
                return;
            }

            await _dynamoDBContext.DeleteAsync<ChatContact>(orderId, chatContact.LastChatTimestamp);
        }
        catch (Exception ex)
        {
            LoggerHelper.LogError(_logger, ex);
            throw new Exception("Error deleting ChatContact", ex); 
        } 
    } 
}
