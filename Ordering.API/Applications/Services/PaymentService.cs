using Core.Configurations;
using Core.Utilities; 
using Microsoft.Extensions.Options; 
using Ordering.API.Applications.Services.Interfaces;

namespace Ordering.API.Applications.Services;

public class PaymentService : IPaymentService
{
    private const int PaymentTimeout = 86400;
    private readonly SenangPayPaymentConfiguration _senangPayConfig;
    public PaymentService(
        IOptionsMonitor<SenangPayPaymentConfiguration> senangPayConfig)
    {
        _senangPayConfig = senangPayConfig.CurrentValue;
    }

    public string GeneratePaymentUrl(
        Guid orderId,
        string detail,
        decimal amount,
        string name,
        string email,
        string phoneNumber)
    {
        var host = _senangPayConfig.Host;
        var merchantId = _senangPayConfig.MerchantId;
        var secret = _senangPayConfig.SecretKey;

        var hash = SecretGenerator.HmacHash($"{secret}{detail}{amount:F2}{orderId}", secret);
         
        var fullPaymentUrl = 
            $"{host}/{merchantId}?" +
            $"detail={Uri.EscapeDataString(detail)}" +
            $"&amount={amount:F2}" +
            $"&order_id={orderId}" +
            $"&name={Uri.EscapeDataString(name)}" +
            $"&email={Uri.EscapeDataString(email ?? string.Empty)}" +
            $"&phone={Uri.EscapeDataString(phoneNumber ?? string.Empty)}" +
            $"&hash={hash}" +
            $"&timeout={PaymentTimeout}";

        return fullPaymentUrl;
    }

    public bool ValidatePayment(Guid orderId, string detail, decimal amount, string hash)
    {
        var secret = _senangPayConfig.SecretKey;
        var comparerHash = SecretGenerator.HmacHash($"{secret}{detail}{amount:F2}{orderId}", secret);
        if (comparerHash == hash)
        {
            return true;
        }

        return false;
    }
}
