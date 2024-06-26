namespace Binbi.Parser.API.Models.Request;

public class ParseByQueryRequest
{
    /// <summary>
    /// Query string for parsing
    /// </summary>
    public string Query { get; set; }
    /// <summary>
    /// Report type
    /// </summary>
    public string TypeReport { get; set; }
}