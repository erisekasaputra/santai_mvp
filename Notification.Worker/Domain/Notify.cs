using Amazon.DynamoDBv2.DataModel;
using Notification.Worker.Enumerations;

namespace Notification.Worker.Domain;

[DynamoDBTable("Notification")]
public class Notify
{
    [DynamoDBHashKey]
    public string NotificationId { get; set; }
    public string BelongsTo { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    [DynamoDBRangeKey]
    public long Timestamp { get; set; }
    [DynamoDBVersion]
    public int? Version { get; set; }
    public Notify()
    {
        NotificationId = Guid.NewGuid().ToString();
        BelongsTo = Guid.NewGuid().ToString();
        Type = NotifyType.Information.ToString();
        Title = "Default";
        Body = "Default";
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public Notify( 
        string belongsTo,
        string type,
        string title,
        string body)
    { 
        NotificationId = Guid.NewGuid().ToString();
        BelongsTo = belongsTo;
        Type = type;
        Title = title;
        Body = body;  
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }   
}
