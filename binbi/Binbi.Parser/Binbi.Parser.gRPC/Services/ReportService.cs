using Binbi.Parser.Common;
using Grpc.Core;

namespace Binbi.Parser.Services;

/// <summary>
/// Report service
/// </summary>
public class ReportService : Report.ReportBase
{
    private readonly ParserService _parserService;
    private readonly ILogger<ReportService> _logger;
    
    /// <summary>
    /// Initialize report service
    /// </summary>
    /// <param name="parserService"></param>
    /// <param name="logger"></param>
    public ReportService(ParserService parserService, ILogger<ReportService> logger)
    {
        _parserService = parserService;
        _logger = logger;
    }

    /// <summary>
    /// Get report by query
    /// </summary>
    /// <param name="getReportRequest"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<ReportReply> GetReport(GetReportRequest getReportRequest, ServerCallContext context)
    {
        // Запрос на ИИ на проверку отчёта
        _logger.LogInformationEx("Checking the availability of a ready-made report");
        
        // Если отчёта нет, то запускаем парсер
        if (true)
        {
            _logger.LogInformationEx("The report was not found. Request to create a new one...");
            
            var parseReply = await _parserService.ParseByQuery(new ParseRequest { Query = getReportRequest.Query }, context);
            
            var toAi = parseReply.ToAiReportModel(getReportRequest.TypeReport);
            // Отсылаем запрос на генерацию
        }
        
        _logger.LogInformationEx("Returning the report...");
        
        var reply = new ReportReply
        {
            Title = "Работает"
        };
        
        return reply;
    }
}