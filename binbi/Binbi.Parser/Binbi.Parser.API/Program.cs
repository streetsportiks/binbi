using Binbi.Parser.API.Services;
using Binbi.Parser.API.Workers;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Binbi.Parser.API", Version = "v1" });
        
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "Binbi.Parser.API.xml");
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
    else
    {
        Console.WriteLine($"XML documentation file not found at path: {xmlPath}");
    }
});

builder.Services.AddCors(options => options.AddPolicy("AllowAllOrigins", builder =>
{
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
    builder.AllowAnyOrigin();
}));

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
builder.Services.AddSingleton<ReportService>();

var app = builder.Build();

app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();