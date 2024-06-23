using Binbi.Parser.Common;
using Binbi.Parser.Workers;
using Grpc.Core;

namespace Binbi.Parser.Services
{
    /// <summary>
    /// Parser service
    /// </summary>
    public class ParserService : Parser.ParserBase
    {
        private readonly ILogger<ParserService> _logger;

        private readonly RbcWorker _rbcWorker;
        private readonly TAdviserWorker _tAdviserWorker;
        private readonly CnewsWorker _cnewsWorker;

        /// <summary>
        /// Initialize parser service
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="tAdviserWorker"></param>
        /// <param name="cnewsWorker"></param>
        /// <param name="rbcWorker"></param>
        public ParserService(ILogger<ParserService> logger, IConfiguration configuration, RbcWorker rbcWorker, TAdviserWorker tAdviserWorker, CnewsWorker cnewsWorker)
        {
            _logger = logger;
            _rbcWorker = rbcWorker;
            _tAdviserWorker = tAdviserWorker;
            _cnewsWorker = cnewsWorker;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 YaBrowser/24.4.0.0 Safari/537.36");
        }

        /// <summary>
        /// Parses sites by query string
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ParseReply> ParseByQuery(ParseRequest request, ServerCallContext context)
        {
            _logger.LogInformationEx("Parsing started...");

            var articles = new List<Article>();
            
            var rbcArticles = await GetArticlesAsync(_rbcWorker, request.Query, request.TypeReport);
            if (rbcArticles != null)
            {
                articles.AddRange(rbcArticles);
            }

            var tAdviserArticles = await GetArticlesAsync(_tAdviserWorker, request.Query, request.TypeReport);
            if (tAdviserArticles != null)
            {
                articles.AddRange(tAdviserArticles);
            }
            
            var cnewsArticles = await GetArticlesAsync(_cnewsWorker, request.Query, request.TypeReport);
            if (cnewsArticles != null)
            {
                articles.AddRange(cnewsArticles);
            }

            var reply = new ParseReply
            {
                TotalCount = articles.Count
            };
            reply.Articles.AddRange(articles);

            return reply;
        }

        private async Task<List<Article>?> GetArticlesAsync(BaseWorker worker, string query, string reportType)
        {
            try
            {
                return await worker.GetArticlesAsync(query, reportType);
            }
            catch (Exception ex)
            {
                _logger.LogErrorEx("An error occurred while receiving the articles", ex);
                return null;
            }
        }
    }
}
