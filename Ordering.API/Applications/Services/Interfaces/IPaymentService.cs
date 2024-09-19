namespace Ordering.API.Applications.Services.Interfaces;

public interface IPaymentService
{
    string GeneratePaymentUrl(
        Guid orderId,
        string orderDetail,
        string name,
        string? email,
        string? phoneNumber,
        decimal amount);
}
