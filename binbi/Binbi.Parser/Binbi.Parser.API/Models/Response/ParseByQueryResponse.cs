using Binbi.Parser.DB.Models;

namespace Binbi.Parser.API.Models.Response;

public class ParseByQueryResponse
{
    /// <summary>
    /// Parsed article list
    /// </summary>
    public List<Article>? Articles { get; set; }
    /// <summary>
    /// Total parsed articles count
    /// </summary>
    public int TotalCount { get; set; }
}