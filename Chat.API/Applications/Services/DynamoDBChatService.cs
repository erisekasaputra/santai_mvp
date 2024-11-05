using Amazon; 
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel; 
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
    //private readonly AmazonDynamoDBClient _dynamoDbClient;
    private readonly IDynamoDBContext _dynamoDBContext; 

    public DynamoDBChatService( 
        IEncryptionService encryptionService,
        IConfiguration configuration,
        IOptionsMonitor<AWSIAMConfiguration> awsIamConfiguration,
        IDynamoDBContext dynamoDBContext)
    {
        //var iam = awsIamConfiguration.CurrentValue; 
        //var credentials = new BasicAWSCredentials(iam.AccessID, iam.SecretKey); 
        //var regionEndpoint = RegionEndpoint.GetBySystemName(iam.Region);
        //_dynamoDbClient = new AmazonDynamoDBClient(credentials, regionEndpoint);
        _encryptionService = encryptionService;
        _dynamoDBContext = dynamoDBContext; 
    }

    public async Task<bool> SaveChatMessageAsync(Conversation conversation)
    { 
        var chatContact = await GetChatContactByOrderId(conversation.OrderId) ?? throw new InvalidOperationException("Chat session is no longer available");

        if (chatContact.IsExpired())
        {
            await _dynamoDBContext.SaveAsync(chatContact);
            throw new InvalidOperationException("Chat session is no longer available");
        }

        //if (string.IsNullOrEmpty(chatContact.MechanicId))
        //{
        //    throw new InvalidOperationException("Waiting for mechanic assignment");
        //}

        try
        {
            conversation.Text = string.IsNullOrEmpty(conversation.Text)
                ? string.Empty
                : await _encryptionService.EncryptAsync(conversation.Text); 
        }
        catch (Exception)
        {
            conversation.Text = string.Empty; 
        }

        try
        {
            conversation.ReplyMessageText = string.IsNullOrEmpty(conversation.ReplyMessageText)
                ? string.Empty
                : await _encryptionService.EncryptAsync(conversation.ReplyMessageText);
        }
        catch (Exception)
        {
            conversation.ReplyMessageText = string.Empty;
        }

        chatContact.UpdateLastChat(conversation.OriginUserId, conversation.Text);

        try
        { 
            await _dynamoDBContext.SaveAsync(conversation);
            await _dynamoDBContext.SaveAsync(chatContact);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to save conversation into DynamoDB", ex);
        }
         
    }

    public async Task<List<Conversation>?> GetMessageByOrderIdAndTimestamp(string orderId, long timestamp, bool forward = true)
    {
        // Configure QueryConfig
        var queryConfig = new QueryOperationConfig
        { 
            IndexName = "OrderId-Timestamp-index",
            KeyExpression = new Expression
            {
                ExpressionStatement = $"#orderId = :orderId AND #timestamp {(forward ? ">" : "<")} :timestamp",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    { "#orderId", nameof(Conversation.OrderId) },
                    { "#timestamp", nameof(Conversation.Timestamp) }
                },
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                {
                    { ":orderId", orderId.ToString() },
                    { ":timestamp", timestamp }
                }
            },
            BackwardSearch = !forward, // Set the order of scanning
            Limit = 100
        };

        try
        {
            // Perform query using QueryAsync with QueryConfig
            var search = _dynamoDBContext.FromQueryAsync<Conversation>(queryConfig);
            var results = await search.GetNextSetAsync();

            // Decrypt message text after retrieving the data
            var tasks = results.Select(async conversation =>
            {
                try
                {
                    conversation.Text = string.IsNullOrEmpty(conversation.Text)
                        ? string.Empty
                        : await _encryptionService.DecryptAsync(conversation.Text);

                    if (!string.IsNullOrEmpty(conversation.ReplyMessageText))
                    {
                        conversation.ReplyMessageText = await _encryptionService.DecryptAsync(conversation.ReplyMessageText);
                    }
                }
                catch (Exception)
                {
                    conversation.Text = string.Empty;
                    conversation.ReplyMessageText = string.Empty;
                }

                return conversation;
            });

            return [.. (await Task.WhenAll(tasks))];
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving messages", ex);
        }
         
    }



    public async Task<bool> CreateChatContact(ChatContact chatContact)
    {
        try
        {
            chatContact.LastChatText = string.IsNullOrEmpty(chatContact.LastChatText)
                ? string.Empty
                : await _encryptionService.EncryptAsync(chatContact.LastChatText);
        }
        catch (Exception)
        {
            chatContact.LastChatText = string.Empty;
        }

        try
        {
            // Save the ChatContact object directly to DynamoDB
            await _dynamoDBContext.SaveAsync(chatContact);
            return true;
        }
        catch (Exception ex)
        {
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
            BackwardSearch = false,
            Limit = 1000
        };

        try
        {
            var search = _dynamoDBContext.FromQueryAsync<ChatContact>(queryConfig);
            var results = await search.GetNextSetAsync();
             
            var decryptedResults = new List<ChatContact>();
            foreach (var chatContact in results)
            { 
                try
                {
                    if (!string.IsNullOrEmpty(chatContact.LastChatText))
                    {
                        chatContact.LastChatText = await _encryptionService.DecryptAsync(chatContact.LastChatText);
                    }
                }
                catch (Exception)
                {
                    chatContact.LastChatText = string.Empty; 
                }

                chatContact.IsExpired(); // this is important, dont remove
                decryptedResults.Add(chatContact);
            }

            return decryptedResults;
        }
        catch (Exception ex)
        {
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
            BackwardSearch = false,
            Limit = 1000
        };

        try
        {
            var search = _dynamoDBContext.FromQueryAsync<ChatContact>(queryConfig);
            var results = await search.GetNextSetAsync();
             
            var decryptedResults = new List<ChatContact>();
            foreach (var chatContact in results)
            {
                try
                {
                    if (!string.IsNullOrEmpty(chatContact.LastChatText))
                    {
                        chatContact.LastChatText = await _encryptionService.DecryptAsync(chatContact.LastChatText);
                    }
                }
                catch (Exception)
                {
                    chatContact.LastChatText = string.Empty;
                }

                chatContact.IsExpired(); // dont remove , this is important
                decryptedResults.Add(chatContact);
            }

            return decryptedResults;
        }
        catch (Exception ex)
        {
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

            if (chatContact != null && !string.IsNullOrEmpty(chatContact.LastChatText))
            {
                try
                {
                    chatContact.LastChatText = await _encryptionService.DecryptAsync(chatContact.LastChatText);
                }
                catch (Exception)
                {
                    chatContact.LastChatText = string.Empty;
                }
            }

            chatContact?.IsExpired(); 
            return chatContact;
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving ChatContact by OrderId", ex);
        }
    }

    public async Task<bool> UpdateChatContact(ChatContact chatContact)
    {
        try
        { 
            if (!string.IsNullOrEmpty(chatContact.LastChatText))
            {
                chatContact.LastChatText = await _encryptionService.EncryptAsync(chatContact.LastChatText);
            }
             
            var existingChatContact = await _dynamoDBContext.LoadAsync<ChatContact>(chatContact.OrderId, chatContact.LastChatTimestamp);
            if (existingChatContact == null)
            {
                return false;
            }
             
            chatContact.IsExpired(); // dont remove this line
            await _dynamoDBContext.SaveAsync(chatContact);
            return true;
        }
        catch(Exception ex)
        {
            throw new Exception("Error updating ChatContact", ex);
        }
    }

    public async Task DeleteChatContact(string orderId)
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
}
