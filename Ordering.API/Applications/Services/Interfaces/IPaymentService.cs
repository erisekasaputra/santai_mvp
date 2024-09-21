using Ordering.API.Applications.Dtos.Requests; 

namespace Ordering.API.Applications.Services.Interfaces;

public interface IPaymentService
{
    bool ValidatePayment(
        SenangPayPaymentRequest request,
        string hash);

    string GeneratePaymentUrl(
        string detail,
        decimal amount,
        Guid orderId,
        string name,
        string email,
        string phoneNumber);
}
