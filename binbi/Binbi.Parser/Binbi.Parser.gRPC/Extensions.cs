namespace Binbi.Parser;

internal static class Extensions
{
    internal static long ConvertToTimestamp(DateTime value)
    {
        var epoch = (value.Ticks - 621355968000000000) / 10000000;
        return epoch;
    }

    internal static bool IsNullOrEmpty(this string? data)
    {
        return string.IsNullOrEmpty(data);
    }

    internal static List<DB.Models.Article> ToDbArticles(this IEnumerable<Article> articles)
    {
        return articles.Select(article => new DB.Models.Article
            {
                ArticleUrl = article.ArticleUrl,
                Data = article.Data,
                Description = article.Description,
                PublishDate = DateTime.Parse(article.PublishDate),
                PublishDateTimeStamp = article.PublishDateTimeStamp,
                Title = article.Title
            }).ToList();
    }
}