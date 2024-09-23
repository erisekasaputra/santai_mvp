using Core.Extensions; 
namespace Ordering.API.Applications.Dtos.Requests;

public class OrderSecretRequest
{
    public required string Secret { get; set; } 
    public OrderSecretRequest(string secret)
    {
        Secret = secret.Clean();
    }
}
