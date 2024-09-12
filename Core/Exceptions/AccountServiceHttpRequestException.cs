using System.Net;

namespace Core.Exceptions;

public class AccountServiceHttpRequestException : HttpRequestException
{
    // Custom property to hold additional information, if needed
    public string? AccountServiceUrl { get; }

    // Constructor that takes only a message
    public AccountServiceHttpRequestException(string message)
        : base(message)
    {
    }

    // Constructor that takes a message and an inner exception
    public AccountServiceHttpRequestException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Constructor that takes a message, inner exception, and additional custom data
    public AccountServiceHttpRequestException(string message, Exception inner, string accountServiceUrl)
        : base(message, inner)
    {
        AccountServiceUrl = accountServiceUrl;
    }

    // Constructor that takes a message and an HTTP status code
    public AccountServiceHttpRequestException(string message, HttpStatusCode statusCode)
        : base(message, null, statusCode)
    {
    }
}
