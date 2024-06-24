using System.Globalization;
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

    internal static IEnumerable<AiArticleModel> ToAiArticleModels(this IEnumerable<Article> articles, string reportType)
    {
        return articles.Select(replyArticle => new AiArticleModel
            {
                Title = replyArticle.Title,
                Description = replyArticle.Description,
                Content = replyArticle.Data,
                Date = replyArticle.PublishDate,
                TypeReport = reportType,
                Url = replyArticle.ArticleUrl
            });
    }

    internal static ReportReply ToReportReply(this AiReportModel reportModel)
    {
        return new ReportReply
        {
            Id = reportModel.Id,
            Title = reportModel.Title ?? string.Empty,
            Description = reportModel.Description ?? string.Empty,
            Created = reportModel.Created ?? string.Empty,
            Updated = reportModel.Updated ?? string.Empty,
            NumberOfSources = reportModel.NumberOfSources,
            Language = reportModel.Language ?? string.Empty,
            TypeReport = reportModel.TypeReport ?? string.Empty,
            ReportTitle = reportModel.ReportTitle ?? string.Empty,
            ReportIntroduction = reportModel.ReportIntroduction ?? string.Empty,
            MarketSegmentation = reportModel.MarketSegmentation ?? string.Empty,
            MarketSize = reportModel.MarketSize ?? string.Empty,
            KeyPlayers = reportModel.KeyPlayers ?? string.Empty,
            ConsumerDemographics = reportModel.ConsumerDemographics ?? string.Empty,
            MarketTrends = reportModel.MarketTrends ?? string.Empty,
            MarketOpportunities = reportModel.MarketOpportunities ?? string.Empty,
            ReportConclusion = reportModel.ReportConclusion ?? string.Empty
        };
    }
}