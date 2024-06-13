using System.Globalization;
using System.Text;
using Binbi.Parser.Models;
using HtmlAgilityPack;

namespace Binbi.Parser.Workers;

internal class CnewsWorker : BaseWorker
{
    private const string BaseUrl = "https://www.cnews.ru";

    public CnewsWorker(ILogger logger, IConfiguration configuration) : base(logger, configuration)
    {
        Logger.LogInformation("Cnews worker has been initialized");
    }
    
    protected override async Task<List<Article>?> GetSearchResultsAsync(string query)
    {
        Logger.LogInformation($"getting items by search query: {query}...");

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

                var date = DateTime.Parse(dataNode.InnerText);
            
                var article = new Article
                {
                    Title = aNode.InnerText,
                    ArticleUrl = "https:" + aNode.GetAttributeValue("href", string.Empty),
                    PublishDate = date.ToString(CultureInfo.CurrentCulture),
                    PublishDateTimeStamp = Extensions.ConvertToTimestamp(date),
                    Description = string.Empty
                };

                articles.Add(article);
            }

            items += 50;
            await Task.Delay(500);
        }
        
        Logger.LogInformation($"search items count: {articles.Count}");

        return articles;
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