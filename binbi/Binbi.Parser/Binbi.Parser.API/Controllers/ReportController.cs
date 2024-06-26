using Binbi.Parser.API.Models.Request;
using Binbi.Parser.API.Models.Response;
using Binbi.Parser.API.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Binbi.Parser.API.Controllers;

[Route("api/v1/report")]
[Produces("application/json")]
[ApiController]
public class ReportController(ReportService reportService) : Controller
{
    /// <summary>
    /// Get report by query
    /// </summary>
    [HttpGet]
    [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(GetReportResponse))]
    public async Task<IActionResult> GetReportAsync([FromQuery] GetReportRequest request)
    {
        var report = await reportService.GetReportAsync(request);

        return Json(report);
    }
}