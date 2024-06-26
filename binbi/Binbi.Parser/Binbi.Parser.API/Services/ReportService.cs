using System.Net;
using Binbi.Parser.API.Models;
using Binbi.Parser.API.Models.Request;
using Binbi.Parser.API.Models.Response;
using Binbi.Parser.Common;

namespace Binbi.Parser.API.Services;

/// <summary>
/// Report service
/// </summary>
public class ReportService
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
    /// <param name="getReportRequest">Request args</param>
    /// <returns><see cref="GetReportResponse"/></returns>
    public async Task<GetReportResponse?> GetReportAsync(GetReportRequest getReportRequest)
    {
        var actualReport = await GetReportAsync(getReportRequest.TypeReport, getReportRequest.Language);
        if (actualReport.Updated.IsNullOrEmpty() && actualReport.Created.TryParseDate() > DateTime.Now.AddDays(-7))
            return actualReport.ToReportResponse();
        
        await _parserService.ParseByQueryAsync(new ParseByQueryRequest
        {
            Query = getReportRequest.Query, 
            TypeReport = getReportRequest.TypeReport
        });

        var report = new AiReportModel();

        try
        {
            if (!await CreateReportAsync(getReportRequest.TypeReport, getReportRequest.Title,
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
        
        return report.ToReportResponse();
    }

    private async Task<bool> CreateReportAsync(string typeReport, string title, string? description)
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