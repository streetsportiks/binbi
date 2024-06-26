namespace Binbi.Parser.API.Models.Response;

public class GetReportResponse
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public int NumberOfSources { get; set; }
    public string Language { get; set; }
    public string TypeReport { get; set; }
    public string ReportTitle { get; set; }
    public string ReportIntroduction { get; set; }
    public string MarketSegmentation { get; set; }
    public string MarketSize { get; set; }
    public string KeyPlayers { get; set; }
    public string ConsumerDemographics { get; set; }
    public string MarketTrends { get; set; }
    public string MarketOpportunities { get; set; }
    public string ReportConclusion { get; set; }
}