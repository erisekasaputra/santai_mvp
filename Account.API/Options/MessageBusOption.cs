namespace Account.API.Options;

public class MessageBusOption
{
    public const string SectionName = "MessageBus";
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int DuplicateDetectionWindows { get; set; }
    public int QueryDelay { get; set; }
    public int QueryTimeout { get; set; }
    public int QueryMessageLimit { get; set; }
    public int MessageRetryInterval { get; set; }
    public int MessageRetryTimespan { get; set; }
    public int MessageTimeout { get; set; }  
    public int DelayedRedeliveryInternval { get; set; } 
}
