﻿using Binbi.Parser.Common;
using Binbi.Parser.DB.DbClient;
using Binbi.Parser.DB.Models;
using Binbi.Parser.Helpers;
using Binbi.Parser.Models;
using MongoDB.Driver;

namespace Binbi.Parser.Workers;

/// <summary>
/// Base worker class for parses
/// </summary>
public abstract class BaseWorker
{
    /// <summary>
    /// Logger who will log worker actions
    /// </summary>
    protected readonly ILogger Logger;
    
    private MongoDbClient? _mongoDbClient;
    private HttpClient _httpClient;
    
    private readonly bool _saveToDb;
    private readonly int _totalItemsCount;
    private readonly string _aiBaseUrl;

    protected BaseWorker(ILogger logger, IConfiguration configuration, HttpClient httpClient)
    {
        Logger = logger;
        _httpClient = httpClient;

        _saveToDb = configuration.GetValue<bool>("ParserOptions:SaveToDb");
        _totalItemsCount = configuration.GetValue<int>("ParserOptions:TotalItemsCount");
        _aiBaseUrl = configuration.GetValue<string>("ConnectionStrings:AiBaseUrl")!;
        
        Logger.LogInformationEx($"Configuration:\nSaveToDb - '{_saveToDb}'\nTotalItemsCount - '{_totalItemsCount}'");
    }

    /// <summary>
    /// Get article list by query
    /// </summary>
    /// <param name="query">Query</param>
    /// <param name="reportType"></param>
    /// <returns><see cref="Article"/> list</returns>
    public async Task<List<Article>?> GetArticlesAsync(string query, string reportType)
    {
        if (_saveToDb && _mongoDbClient is not null && !await _mongoDbClient.CheckDbConnectionAsync())
        {
            Logger.LogErrorEx("Request has been aborted");
            return null;
        }
        
        Logger.LogInformationEx("Start getting articles...");
        
        var searchModel = await GetSearchResultsAsync(query);
        if (searchModel == null) return default;
        
        
        var articles = await GetArticlesAsync(searchModel);
        
        Logger.LogInformationEx("Articles successfully get!");

        if (_saveToDb && articles is not null && articles.Count > 0)
        {
            await LoadToAiAsync(articles.ToAiArticleModels(reportType));
            await SaveToDbAsync(query, articles);
        }

        return articles;
    }

    /// <summary>
    /// Init mongo db client
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns><see cref="MongoDbClient"/></returns>
    /// <exception cref="MongoException">When connection data is null</exception>
    protected void InitMongoDb(IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string?>("ConnectionStrings:MongoDB:ConnectionString");
        var dbName = configuration.GetValue<string?>("ConnectionStrings:MongoDB:DbName");

        if (connectionString.IsNullOrEmpty() || dbName.IsNullOrEmpty())
        {
            throw new MongoException("MongoDB 'connectionString' or 'dbName' is null. Check app configuration and repeate request");
        }

        _mongoDbClient = new MongoDbClient(connectionString!, dbName!, Logger);
    }

    private async Task LoadToAiAsync(IEnumerable<AiArticleModel> articles)
    {
        try
        {
            var existArticles = await _httpClient.GetFromJsonAsync<List<AiArticleModel>>(_aiBaseUrl + "/api/v1/article/findAllArticles");

            if (existArticles is null || existArticles.Count == 0)
            {
                Logger.LogWarnEx("Empty articles for load to ai");
                return;
            }
            
            foreach (var article in articles)
            {
                if (existArticles.Select(a => a.Url).Contains(article.Url))
                    continue;

                await _httpClient.PostAsJsonAsync(_aiBaseUrl + "/api/v1/article/addArticle", article);
            }
        }
        catch (Exception ex)
        {
            Logger.LogErrorEx("An error has occurred", ex);
        }
    }
    
    private async Task SaveToDbAsync(string query, IReadOnlyCollection<Article> articles)
    {
        try
        {
            await _mongoDbClient!.SaveArticlesAsync(new Category
            {
                Name = query,
                Articles = articles.ToDbArticles()
            });
        }
        catch (Exception ex)
        {
            Logger.LogErrorEx("An error has occurred", ex);
        }
    }

    /// <summary>
    /// Get search results from site by query
    /// </summary>
    /// <param name="query">Query string</param>
    /// <returns><see cref="Article"/> list</returns>
    protected abstract Task<List<Article>?> GetSearchResultsAsync(string query);
    /// <summary>
    /// Get articles from HTML document
    /// </summary>
    /// <param name="articles">Found articles</param>
    /// <returns><see cref="Article"/> list</returns>
    protected abstract Task<List<Article>?> GetArticlesAsync(List<Article> articles);

    /// <summary>
    /// Parse HTML document
    /// </summary>
    /// <param name="articles"></param>
    /// <param name="parseAction"></param>
    protected async Task ParseArticlesAsync(List<Article> articles, Func<Article, Task> parseAction)
    {
        Logger.LogInformationEx("getting props...");

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
                    //progressBar.Report(currentIteratorValue);
                    
                    await parseAction(item);
                    await Task.Delay(random.Next(400, 600), cancellationToken);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // ignored
                }
                catch (Exception ex)
                {
                    Logger.LogErrorEx("An error has occurred", ex);
                }
                finally
                {
                    semaphore.Release();
                }
            });

        Logger.LogInformationEx($"count parsed articles: {articles.Count}");
    }
}