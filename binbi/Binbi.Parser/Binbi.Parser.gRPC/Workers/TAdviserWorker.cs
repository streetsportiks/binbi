using System.Globalization;
using System.Text;
using Binbi.Parser.Common;
using HtmlAgilityPack;

namespace Binbi.Parser.Workers;

/// <summary>
/// The worker who parses the site https://www.tadviser.ru
/// </summary>
public class TAdviserWorker : BaseWorker
{
    private const string BaseUrl = "https://www.tadviser.ru";
    
    /// <summary>
    /// Initialize worker
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public TAdviserWorker(ILogger<TAdviserWorker> logger, IConfiguration configuration) : base(logger, configuration)
    {
        Logger.LogInformationEx("TAdviser worker has been initialized");
        
        var saveToDb = configuration.GetValue<bool>("ParserOptions:SaveToDb");
        if (saveToDb) InitMongoDb(configuration);
    }

    /// <inheritdoc />
    protected override async Task<List<Article>?> GetSearchResultsAsync(string query)
    {
        Logger.LogInformationEx($"getting items by search query: {query}...");
        
        var web = new HtmlWeb();
        var htmlDoc = await web.LoadFromWebAsync($"{BaseUrl}/index.php/Служебная:Search?ns30=1&ns132=1&redirs=0&search={query}&limit=500&offset=0");
        var liNodes = htmlDoc.DocumentNode.SelectNodes("//li");

        var articles = new List<Article>();

        if (liNodes == null) return new List<Article>();
        
        foreach (var li in liNodes)
        {
            var aNode = li.SelectSingleNode(".//a");
            var dataNode = li.SelectSingleNode(".//div[@class='mw-search-result-data']");

            if (aNode == null || dataNode == null) continue;

            var date = ExtractDate(dataNode.InnerText);

            articles.Add(new Article
            {
                Title = aNode.GetAttributeValue("title", string.Empty),
                ArticleUrl = BaseUrl + aNode.GetAttributeValue("href", string.Empty),
                Description = string.Empty,
                PublishDate = date.ToString(CultureInfo.CurrentCulture),
                PublishDateTimeStamp = date.ConvertToTimestamp()
            });
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
    
    private static DateTime ExtractDate(string data)
    {
        var index = data.IndexOf("- ", StringComparison.Ordinal);

        if (index != -1)
        {
            var dateString = data[(index + 2)..].Trim();
            var formats = new[] { "MM/dd/yyyy HH:mm:ss", "dd.MM.yyyy HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "dd/MM/yyyy HH:mm:ss", "HH:mm, dd MMMM yyyy" };
        
            if (DateTime.TryParseExact(dateString, formats, new CultureInfo("ru-RU"), DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }
        }

        return default;
    }
}