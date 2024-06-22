using Binbi.Parser.Common;
using Binbi.Parser.Models;

namespace Binbi.Parser;

internal static class Extensions
{
    internal static List<DB.Models.Article> ToDbArticles(this IEnumerable<Article> articles)
    {
        return articles.Select(article => new DB.Models.Article
            {
                ArticleUrl = article.ArticleUrl,
                Data = article.Data,
                Description = article.Description,
                PublishDate = article.PublishDate.TryParseDate(),
                PublishDateTimeStamp = article.PublishDateTimeStamp,
                Title = article.Title
            }).ToList();
    }

    internal static List<AiReportModel> ToAiReportModel(this ParseReply parseReply, string reportType)
    {
        return parseReply.Articles.Select(replyArticle => new AiReportModel
            {
                Title = replyArticle.Title,
                Description = replyArticle.Description,
                Content = replyArticle.Data,
                Date = replyArticle.PublishDate,
                TypeReport = reportType,
                Url = replyArticle.ArticleUrl
            }).ToList();
    }
}