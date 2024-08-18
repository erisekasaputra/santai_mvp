namespace Identity.API.Configs;

public class EventBusConfig
{
    public const string SectionName = "EventBus";
    public required string Host { get; set; } 
    public required string Username { get; set; } 
    public required string Password { get; set; } 
    public int DuplicateDetectionWindows { get; set; }
    public int QueryDelay { get; set; }
    public int QueryTimeout { get; set; }
    public int QueryMessageLimit { get; set; }
    public int MessageRetryInterval { get; set; }
    public int MessageRetryTimespan { get; set; }
    public int MessageTimeout { get; set; }
}
