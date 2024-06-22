namespace Binbi.Parser.Models;

/// <summary>
/// Модель отчёта для его загрузки в ИИ
/// </summary>
public class AiReportModel
{
    public string Title {get;set;}
    public string Description {get;set;}
    public string Url {get;set;}
    public string Date {get;set;}
    public string Content {get;set;}
    public string TypeReport {get;set;}
}