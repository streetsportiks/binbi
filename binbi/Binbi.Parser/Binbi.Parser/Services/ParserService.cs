﻿using Binbi.Parser.Workers;
using Grpc.Core;

namespace Binbi.Parser.Services
{
    public class ParserService : Parser.ParserBase
    {
        private readonly ILogger<ParserService> _logger;
        private readonly HttpClient _httpClient;

        private readonly RbcWorker _rbcWorker;
        private readonly TAdviserWorker _tAdviserWorker;

        public ParserService(ILogger<ParserService> logger, IConfiguration configuration)
        {
            _logger = logger;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 YaBrowser/24.4.0.0 Safari/537.36");

            _rbcWorker = new RbcWorker(_logger, _httpClient, configuration);
            _tAdviserWorker = new TAdviserWorker(_logger, configuration);
        }

        public override async Task<ParseReply> ParseByQuery(ParseRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Parsing started...");

            var articles = new List<Article>();

            var rbcArticles = await GetArticlesAsync(_rbcWorker, request.Query);
            if (rbcArticles != null)
            {
                articles.AddRange(rbcArticles);
            }

            var tAdviserArticles = await GetArticlesAsync(_tAdviserWorker, request.Query);
            if (tAdviserArticles != null)
            {
                articles.AddRange(tAdviserArticles);
            }

            var reply = new ParseReply
            {
                TotalCount = articles.Count
            };
            reply.Articles.AddRange(articles);

            return reply;
        }

        private async Task<List<Article>?> GetArticlesAsync(BaseWorker worker, string query)
        {
            try
            {
                return await worker.GetArticlesAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting articles: {ex.Message}");
                return null;
            }
        }
    }
}