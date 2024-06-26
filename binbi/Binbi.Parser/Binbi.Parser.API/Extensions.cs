using System.Globalization;
using Binbi.Parser.API.Models;
using Binbi.Parser.API.Models.Response;
using Binbi.Parser.DB.Models;

namespace Binbi.Parser.API;

public static class Extensions
{
    public static List<Article> ToDbArticles(this IEnumerable<Article> articles)
    {
        return articles.Select(article => new Article
            {
                ArticleUrl = article.ArticleUrl,
                Data = article.Data,
                Description = article.Description,
                PublishDate = article.PublishDate,
                PublishDateTimeStamp = article.PublishDateTimeStamp,
                Title = article.Title
            }).ToList();
    }

    public static IEnumerable<AiArticleModel> ToAiArticleModels(this IEnumerable<Article> articles, string reportType)
    {
        return articles.Select(replyArticle => new AiArticleModel
            {
                Title = replyArticle.Title,
                Description = replyArticle.Description,
                Content = replyArticle.Data,
                Date = replyArticle.PublishDate.ToString(CultureInfo.CurrentCulture),
                TypeReport = reportType,
                Url = replyArticle.ArticleUrl
            });
    }

    public static GetReportResponse ToReportResponse(this AiReportModel reportModel)
    {
        return new GetReportResponse
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