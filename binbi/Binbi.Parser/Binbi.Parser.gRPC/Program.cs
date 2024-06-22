using Binbi.Parser.Services;
using Binbi.Parser.Workers;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpcSwagger();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Binbi.Parser.gRPC", Version = "v1" });
        
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Binbi.Parser.gRPC.xml");
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
    else
    {
        Console.WriteLine($"XML documentation file not found at path: {xmlPath}");
    }
});

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog().SetMinimumLevel(LogLevel.Information);
builder.Host.UseSerilog();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<CnewsWorker>();
builder.Services.AddSingleton<RbcWorker>();
builder.Services.AddSingleton<TAdviserWorker>();

builder.Services.AddSingleton<ParserService>();

var app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment()) {
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Binbi.Parser API V1");
    });
}

app.MapGrpcService<ReportService>();
app.MapGrpcService<ParserService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();