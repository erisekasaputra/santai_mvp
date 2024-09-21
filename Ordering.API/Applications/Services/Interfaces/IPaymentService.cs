using Ordering.API.Applications.Dtos.Requests; 

namespace Ordering.API.Applications.Services.Interfaces;

public interface IPaymentService
{
    bool ValidatePayment(
        Guid orderId,
        string detail,
        decimal amount,
        string hash);

    string GeneratePaymentUrl(
        Guid orderId,
        string detail,
        decimal amount,
        string name,
        string email,
        string phoneNumber);
}
