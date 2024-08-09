namespace Account.API.Options;

public class MessageBusOption
{
    public const string SectionName = "MessageBus";
    public string Host { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
