namespace Binbi.Parser.API.Models;

/// <summary>
/// Модель отчёта для его загрузки в ИИ
/// </summary>
public class AiReportModel
{
    public string Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string Created {get; set;} = string.Empty;
    public string Updated { get; set; } = string.Empty;
    public int NumberOfSources {get; set;}
    public string Language {get; set;} = string.Empty;
    public string TypeReport {get; set;} = string.Empty;
    public string ReportTitle {get; set;} = string.Empty;
    public string ReportIntroduction {get; set;} = string.Empty;
    public string MarketSegmentation {get; set;} = string.Empty;
    public string MarketSize {get; set;} = string.Empty;
    public string KeyPlayers {get; set;} = string.Empty;
    public string ConsumerDemographics {get; set;} = string.Empty;
    public string MarketTrends {get; set;} = string.Empty;
    public string MarketOpportunities {get; set;} = string.Empty;
    public string ReportConclusion {get; set;} = string.Empty;
}