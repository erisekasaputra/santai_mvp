using Microsoft.Extensions.Logging;

namespace Core.Utilities;

public static class LoggerHelper
{
    public static void LogError(ILogger logger, Exception ex)
    {
        logger.LogError(ex, "An error occurred, Error: {Error}, Detail: {Detail}", ex.Message, ex.InnerException?.Message);
    } 
}
