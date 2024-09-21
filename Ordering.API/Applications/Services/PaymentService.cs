using Core.Configurations;
using Core.Utilities; 
using Microsoft.Extensions.Options;
using Ordering.API.Applications.Dtos.Requests;
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
        string detail,
        decimal amount,
        Guid orderId,
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

    public bool ValidatePayment(SenangPayPaymentRequest request, string hash)
    {
        var secret = _senangPayConfig.SecretKey; 
        var comparerHash = SecretGenerator.HmacHash($"?transactionId={request.TransactionId}&orderId={request.OrderId}&amount={request.Amount:F2}&method={request.Method}&message=P{request.Message}&status={request.Status}&hash={request.Hash}", secret);
        if (comparerHash == hash)
        {
            return true;
        }

        return false;
    }
}
