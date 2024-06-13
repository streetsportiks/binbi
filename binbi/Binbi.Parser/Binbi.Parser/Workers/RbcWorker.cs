using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;
using Binbi.Parser.Helpers;
using Binbi.Parser.Models;
using HtmlAgilityPack;

namespace Binbi.Parser.Workers;

internal class RbcWorker : BaseWorker
{
    private readonly HttpClient _httpClient;
    internal RbcWorker(ILogger logger, HttpClient httpClient, IConfiguration configuration) : base(logger, configuration)
    {
        Logger.LogInformation("Rbc worker has been initialized");

        _httpClient = httpClient;
    }

    protected override async Task<List<Article>?> GetSearchResultsAsync(string query)
    {
        Logger.LogInformation($"getting items by search query: {query}...");
        
        var rbcSearchModel = new RbcSearchModel { Items = new List<RbcSearchItems>() };
        
        HttpStatusCode statusCode;
        var page = 0;
        
        do
        {
            var response = await _httpClient.GetAsync($"https://www.rbc.ru/search/ajax/?query={query}&page={page}");
            statusCode = response.StatusCode;

            if (response.StatusCode != HttpStatusCode.OK) break;
            
            var contentByteArray = response.Content.ReadAsByteArrayAsync().Result;
            var deserializedRbcSearchModel = JsonSerializer.Deserialize<RbcSearchModel>(Compressor.Decompress(contentByteArray));
            
            if (deserializedRbcSearchModel?.Items is null || deserializedRbcSearchModel.Items.Count == 0)
                continue;
            
            rbcSearchModel.Items.AddRange(deserializedRbcSearchModel.Items);
            page++;

        } while (statusCode == HttpStatusCode.OK);

        Logger.LogInformation($"search items count: {rbcSearchModel.Items.Count}");
        
        return rbcSearchModel.Items.Select(item => new Article
        {
            Title = item.Title,
            ArticleUrl = item.ArticleUrl,
            Description = item.Body ?? string.Empty,
            PublishDate = item.PublishDate,
            PublishDateTimeStamp = item.PublishDateTimeStamp
        }).ToList();
    }

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