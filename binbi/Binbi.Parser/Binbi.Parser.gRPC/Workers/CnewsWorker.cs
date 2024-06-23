using System.Globalization;
using System.Text;
using Binbi.Parser.Common;
using HtmlAgilityPack;

namespace Binbi.Parser.Workers;

/// <summary>
/// The worker who parses the site https://www.cnews.ru
/// </summary>
public class CnewsWorker : BaseWorker
{
    private const string BaseUrl = "https://www.cnews.ru";

    /// <summary>
    /// Initialize worker
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClient"></param>
    /// <param name="configuration"></param>
    public CnewsWorker(ILogger<CnewsWorker> logger, HttpClient httpClient, IConfiguration configuration) : base(logger, configuration, httpClient)
    {
        Logger.LogInformationEx("Cnews worker has been initialized");
        
        var saveToDb = configuration.GetValue<bool>("ParserOptions:SaveToDb");
        if (saveToDb) InitMongoDb(configuration);
    }

    /// <inheritdoc />
    protected override async Task<List<Article>?> GetSearchResultsAsync(string query)
    {
        Logger.LogInformationEx($"getting items by search query: {query}...");

        var articles = new List<Article>();
        var web = new HtmlWeb();
        var items = 0;

        while (true)
        {
            var htmlDoc = await web.LoadFromWebAsync($"{BaseUrl}/search/more/{items}?search={query}");
            
            var divNodes = htmlDoc.DocumentNode.SelectNodes(".//div[@class='allnews_item visible-items']");
            if (divNodes == null) break;
        
            foreach (var li in divNodes)
            {
                var aNode = li.SelectSingleNode(".//a[@class='ani-postname']");
                var dataNode = li.SelectSingleNode(".//span[@class='ani-date']");

                if (aNode == null || dataNode == null) continue;

                var date = dataNode.InnerText.TryParseDate();
            
                var article = new Article
                {
                    Title = aNode.InnerText,
                    ArticleUrl = "https:" + aNode.GetAttributeValue("href", string.Empty),
                    PublishDate = date.ToString(CultureInfo.CurrentCulture),
                    PublishDateTimeStamp = date.ConvertToTimestamp(),
                    Description = string.Empty
                };

                articles.Add(article);
            }

            items += 50;
            await Task.Delay(500);
        }
        
        Logger.LogInformationEx($"search items count: {articles.Count}");

        return articles;
    }

    /// <inheritdoc />
    protected override async Task<List<Article>?> GetArticlesAsync(List<Article> articles)
    {
        var validArticles = new List<Article>();

        await ParseArticlesAsync(articles, async item =>
        {
            var web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(item.ArticleUrl);
            var articleParts = htmlDoc.DocumentNode.SelectNodes("//p");
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