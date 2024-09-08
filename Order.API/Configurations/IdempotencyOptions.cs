namespace Order.API.Configurations;

public class IdempotencyOptions
{
    public const string SectionName = "Idempotency";
    public bool IsActive { get; set; }
    public int TTL { get; set; }
}
