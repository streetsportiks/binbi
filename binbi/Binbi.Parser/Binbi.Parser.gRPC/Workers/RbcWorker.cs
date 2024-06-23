using System.Net;
using System.Text;
using System.Text.Json;
using Binbi.Parser.Common;
using Binbi.Parser.Helpers;
using Binbi.Parser.Models;
using HtmlAgilityPack;

namespace Binbi.Parser.Workers;

/// <summary>
/// The worker who parses the site https://www.rbc.ru
/// </summary>
public class RbcWorker : BaseWorker
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initialize worker
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClient"></param>
    /// <param name="configuration"></param>
    public RbcWorker(ILogger<RbcWorker> logger, HttpClient httpClient, IConfiguration configuration) : base(logger, configuration, httpClient)
    {
        Logger.LogInformationEx("Rbc worker has been initialized");
        
        var saveToDb = configuration.GetValue<bool>("ParserOptions:SaveToDb");
        if (saveToDb) InitMongoDb(configuration);

        _httpClient = httpClient;
    }

    /// <inheritdoc />
    protected override async Task<List<Article>?> GetSearchResultsAsync(string query)
    {
        Logger.LogInformationEx($"getting items by search query: {query}...");
        
        var rbcSearchModel = new RbcSearchModel { Items = new List<RbcSearchItems>() };
        
        var statusCode = HttpStatusCode.NoContent;
        var page = 0;
        
        const int retryCount = 3;
        var attempt = 0;
        
        do
        {
            var response = new HttpResponseMessage();
            
            while (attempt < retryCount)
            {
                response = await _httpClient.GetAsync($"https://www.rbc.ru/search/ajax/?query={query}&page={page}");
                statusCode = response.StatusCode;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Logger.LogInformationEx("Retry get response from RBC...");
                    attempt++;
                }
                else break;
            }
            
            if (response.StatusCode != HttpStatusCode.OK) break;
            
            var contentByteArray = response.Content.ReadAsByteArrayAsync().Result;
            var deserializedRbcSearchModel = JsonSerializer.Deserialize<RbcSearchModel>(contentByteArray);
            
            if (deserializedRbcSearchModel?.Items is null || deserializedRbcSearchModel.Items.Count == 0)
                continue;
            
            rbcSearchModel.Items.AddRange(deserializedRbcSearchModel.Items);
            page++;

        } while (statusCode == HttpStatusCode.OK);

        Logger.LogInformationEx($"search items count: {rbcSearchModel.Items.Count}");
        
        return rbcSearchModel.Items.Select(item => new Article
        {
            Title = item.Title,
            ArticleUrl = item.ArticleUrl,
            Description = item.Body ?? string.Empty,
            PublishDate = item.PublishDate,
            PublishDateTimeStamp = item.PublishDateTimeStamp
        }).ToList();
    }

    /// <inheritdoc />
    protected override async Task<List<Article>?> GetArticlesAsync(List<Article> articles)
    {
        var validArticles = new List<Article>();

        await ParseArticlesAsync(articles, async item =>
        {
            var web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(item.ArticleUrl);
            var articleParts = htmlDoc.DocumentNode.SelectNodes("//div/p");
            var stringBuilder = new StringBuilder();

            if (articleParts != null)
            {
                foreach (var part in articleParts)
                {
                    stringBuilder.Append($"{part.InnerText}\n");
                }
            }

            item.Data = stringBuilder.ToString();

            if (!string.IsNullOrEmpty(item.Data))
            {
                validArticles.Add(item);
            }
        });

        return validArticles;
    }
}