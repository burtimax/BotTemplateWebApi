using MultipleTestBot.Constants;

namespace MultipleTestBot.Extensions;
using static System.String;

public static class ILoggerExtension
{
    public static void LogEndpointCall<T>(this ILogger<T> logger, string endpoint)
        => logger.LogDebug(Format(AppConstants.Logging.CallEndpointFormat, endpoint));
}