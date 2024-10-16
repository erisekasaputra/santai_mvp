namespace Ordering.API.Applications.Dtos.Responses;

public class OrdersActiveResponseDto
{
    public Guid Id { get; set; }
    public string Secret { get; set; }
    public string Status { get; set; }
    public int Step { get; set; }
    public OrdersActiveResponseDto(
        Guid id,
        string secret,
        string status,
        int step)
    {
        Id = id;
        Secret = secret;
        Status = status;
        Step = step;
    }
}

