using Binbi.Parser.API.Models.Request;
using Binbi.Parser.API.Models.Response;
using Binbi.Parser.API.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Binbi.Parser.API.Controllers;

[Route("api/v1/parse")]
[Produces("application/json")]
[ApiController]
public class ParseController(ParserService parserService) : Controller
{
    /// <summary>
    /// Parse articles by query
    /// </summary>
    [HttpGet]
    [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(ParseByQueryResponse))]
    public async Task<IActionResult> ParseByQueryAsync([FromQuery] ParseByQueryRequest request)
    {
        var report = await parserService.ParseByQueryAsync(request);

        return Json(report);
    }
}