using Binbi.Parser.Helpers;

namespace Binbi.Parser.Workers;

internal abstract class BaseWorker(ILogger logger, IConfiguration configuration)
{
    protected readonly ILogger Logger = logger;
    private readonly int _totalItemsCount = configuration.GetValue<int>("ParserOptions:TotalItemsCount");

    public async Task<List<Article>?> GetArticlesAsync(string query)
    {
        Logger.LogInformation("Start getting articles...");
        
        var searchModel = await GetSearchResultsAsync(query);
        if (searchModel == null) return default;
        
        var articles = await GetArticlesAsync(searchModel);
        
        Logger.LogInformation("Articles successfully get!");

        return articles;
    }

    protected abstract Task<List<Article>?> GetSearchResultsAsync(string query);
    protected abstract Task<List<Article>?> GetArticlesAsync(List<Article> articles);

    protected async Task<List<Article>?> ParseArticlesAsync(List<Article> articles, Func<Article, Task> parseAction)
    {
        Logger.LogInformation("getting props...");

        var totalItemsCount = articles.Count > _totalItemsCount ? _totalItemsCount : articles.Count;
        var iterator = 0;
        var semaphore = new SemaphoreSlim(5);
        var random = new Random();
        var progressBar = new ProgressBar(totalItemsCount);

        await Parallel.ForEachAsync(totalItemsCount == articles.Count ? articles : articles.Take(totalItemsCount), 
        async (item, cancellationToken) =>
            {
                await semaphore.WaitAsync(cancellationToken);
                try
                {
                    var currentIteratorValue = Interlocked.Increment(ref iterator);
                    progressBar.Report(currentIteratorValue);
                    await parseAction(item);
                    await Task.Delay(random.Next(400, 600), cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            });

        Logger.LogInformation($"count parsed articles: {articles.Count}");
        return articles.ToList();
    }
}