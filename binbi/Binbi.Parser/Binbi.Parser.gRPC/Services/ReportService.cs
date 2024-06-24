using System.Net;
using Binbi.Parser.Common;
using Binbi.Parser.Models;
using Grpc.Core;

namespace Binbi.Parser.Services;

/// <summary>
/// Report service
/// </summary>
public class ReportService : Report.ReportBase
{
    private readonly ParserService _parserService;
    private readonly ILogger<ReportService> _logger;
    private readonly HttpClient _httpClient;
    
    private readonly string _aiBaseUrl;

    /// <summary>
    /// Initialize report service
    /// </summary>
    /// <param name="parserService"></param>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public ReportService(ParserService parserService, ILogger<ReportService> logger, IConfiguration configuration)
    {
        _parserService = parserService;
        _logger = logger;

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
        httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 YaBrowser/24.4.0.0 Safari/537.36");

        _httpClient = httpClient;
        
        _aiBaseUrl = configuration.GetValue<string>("ConnectionStrings:AiBaseUrl")!;
    }

    /// <summary>
    /// Get report by query
    /// </summary>
    /// <param name="getReportRequest"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<ReportReply?> GetReport(GetReportRequest getReportRequest, ServerCallContext context)
    {
        await _parserService.ParseByQuery(new ParseRequest { Query = getReportRequest.Query }, context);

        var report = new AiReportModel();

        try
        {
            if (!await CreateReportASync(getReportRequest.TypeReport, getReportRequest.Title,
                    getReportRequest.Description))
            {
                return null;
            }

            report = await GetReportAsync(getReportRequest.TypeReport, getReportRequest.Language);
        }
        catch (Exception ex)
        {
            _logger.LogErrorEx("An error was occurred:", ex);
        }
        
        return report.ToReportReply();
    }

    private async Task<bool> CreateReportASync(string typeReport, string title, string description)
    {
        var body = new
        {
            typeReport,
            title,
            description
        };

        _logger.LogInformationEx("Sending a request to create a report");
        
        var response = await _httpClient.PostAsJsonAsync(_aiBaseUrl + "/api/v1/openai/addReport", body);
        if (response.StatusCode != HttpStatusCode.OK)
        {
            _logger.LogErrorEx("The report was not created");
            return false;
        }
        
        _logger.LogInformationEx("The report was created!");
        return true;
    }
    
    private async Task<AiReportModel> GetReportAsync(string reportType, string language)
    {
        _logger.LogInformationEx("Sending a request to receive reports");
        var response = await _httpClient.GetFromJsonAsync<List<AiReportModel>>(_aiBaseUrl + $"/api/v1/report/findReports/{reportType}");
        if (response is null)
        {
            _logger.LogWarnEx("The report was not received from the AI service");
            return new AiReportModel();
        }

        return response.OrderByDescending(r => r.Created).FirstOrDefault(r => string.Equals(r.Language, language, StringComparison.CurrentCultureIgnoreCase)) ?? new AiReportModel();
    }
}