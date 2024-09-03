namespace Order.Infrastructure.SeedWorks;

public class AccountServiceResponseDto<TDataResult>
{
    public bool IsSuccess { get; set; }
    public TDataResult? Data { get; set; }
}
