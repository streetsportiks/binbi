namespace Binbi.Parser;

internal static class Extensions
{
    internal static DateTime ExtractDate(string data)
    {
        var index = data.IndexOf("- ", StringComparison.Ordinal);
        
        return index != -1 ? DateTime.Parse(data[(index + 2)..].Trim()) : default;
    }
    
    internal static long ConvertToTimestamp(DateTime value)
    {
        var epoch = (value.Ticks - 621355968000000000) / 10000000;
        return epoch;
    }
}