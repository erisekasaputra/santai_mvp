using Core.Extensions;

namespace Ordering.API.Applications.Dtos.Requests;

public class OrderSecretRequest
{
    public string Secret { get; private set; } 
    public OrderSecretRequest(string secret)
    {
        Secret = secret.Clean();
    }
}
