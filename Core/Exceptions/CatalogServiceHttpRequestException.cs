using System.Net;

namespace Core.Exceptions;
 
public class CatalogServiceHttpRequestException : HttpRequestException
{
    // Custom property to hold additional information, if needed
    public string? AccountServiceUrl { get; }

    // Constructor that takes only a message
    public CatalogServiceHttpRequestException(string message)
        : base(message)
    {
    }

    // Constructor that takes a message and an inner exception
    public CatalogServiceHttpRequestException(string message, Exception inner)
        : base(message, inner)
    {
    }

    // Constructor that takes a message, inner exception, and additional custom data
    public CatalogServiceHttpRequestException(string message, Exception inner, string accountServiceUrl)
        : base(message, inner)
    {
        AccountServiceUrl = accountServiceUrl;
    }

    // Constructor that takes a message and an HTTP status code
    public CatalogServiceHttpRequestException(string message, HttpStatusCode statusCode)
        : base(message, null, statusCode)
    {
    }
}

