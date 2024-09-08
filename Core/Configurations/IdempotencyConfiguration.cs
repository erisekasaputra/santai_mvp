namespace Core.Configurations;

public class IdempotencyConfiguration
{
    public const string SectionName = "Idempotency";
    public bool IsActive { get; set; }
    public int TTL { get; set; }
}
