using System.Net;

namespace Core.Exceptions; 

public class PaymentServiceHttpRequestException : HttpRequestException
{
    // Custom property to hold additional information, if needed
    public string? AccountServiceUrl { get; }

    // Constructor that takes only a message
    public PaymentServiceHttpRequestException(string message)
        : base(message)
    {
    }

    // Constructor that takes a message and an inner exception
    public PaymentServiceHttpRequestException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Constructor that takes a message, inner exception, and additional custom data
    public PaymentServiceHttpRequestException(string message, Exception inner, string accountServiceUrl)
        : base(message, inner)
    {
        AccountServiceUrl = accountServiceUrl;
    }

    // Constructor that takes a message and an HTTP status code
    public PaymentServiceHttpRequestException(string message, HttpStatusCode statusCode)
        : base(message, null, statusCode)
    {
    }
}

