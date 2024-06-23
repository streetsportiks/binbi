using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Binbi.Parser.Common;

public static class Extensions
{
    private static readonly string[] DateFormats =
    [
        "MM/dd/yyyy HH:mm:ss", 
        "dd.MM.yyyy HH:mm:ss", 
        "yyyy-MM-dd HH:mm:ss", 
        "dd/MM/yyyy HH:mm:ss", 
        "HH:mm, dd MMMM yyyy",
        "dd.MM.yyyy HH:mm",
        "dd.MM.yyyy HH:mm:ss"
    ];

    public static long ConvertToTimestamp(this DateTime value)
    {
        var epoch = (value.Ticks - 621355968000000000) / 10000000;
        return epoch;
    }

    public static bool IsNullOrEmpty(this string? data)
    {
        return string.IsNullOrEmpty(data);
    }
    
    public static DateTime TryParseDate(this string dateString)
    {
        return DateTime.TryParseExact(dateString, DateFormats, new CultureInfo("ru-RU"), DateTimeStyles.None,
            out var parsedDate) ? parsedDate : default;
    }
    
    public static void LogInformationEx(this ILogger logger, string message, Exception? ex = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.LogInformation(ex, "{Message} (at {File}:{LineNumber})", message, file, lineNumber);
    }

    public static void LogErrorEx(this ILogger logger, string message, Exception? ex = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.LogError(ex, "{Message} (at {File}:{LineNumber})", message, file, lineNumber);
    }


    public static void LogWarnEx(this ILogger logger, string message, Exception? ex = null,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logger.LogWarning(ex, "{Message} (at {File}:{LineNumber})", message, file, lineNumber);
    }

}