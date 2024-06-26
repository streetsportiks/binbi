namespace Binbi.Parser.API.Models.Request;

public class GetReportRequest
{
    /// <summary>
    /// Query string fot get report
    /// </summary>
    public string Query { get; set; }
    /// <summary> 
    /// Report type
    /// </summary>
    public string TypeReport { get; set; }
    /// <summary>
    /// Report title
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Report description
    /// </summary>
    /// <remarks>MARKET_OVERVIEW</remarks>
    public string? Description { get; set; }
    /// <summary>
    /// Report language
    /// </summary>
    /// <remarks>russian, english...</remarks>
    public string Language { get; set; }
}