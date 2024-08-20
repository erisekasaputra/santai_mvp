namespace Identity.API.Extensions;

public static class HttpContextExtensions
{
    public static string? GetBearerToken(this HttpContext httpContext)
    {
        if (httpContext == null)
        {
            throw new ArgumentNullException(nameof(httpContext));
        }

        var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return null;
        }

        // Check if the header starts with "Bearer "
        if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }

        // If the header is not in the expected format, return null or throw an exception
        return null;
    }
}