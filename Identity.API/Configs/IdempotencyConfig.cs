namespace Identity.API.Configs;

public class IdempotencyConfig
{
    public const string SectionName = "Idempotency";
    public bool IsActive { get; set; }
    public int TTL { get; set; }
}
