using Binbi.Parser.Common;
using Binbi.Parser.DB.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Binbi.Parser.DB.DbClient;

public class MongoDbClient
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly ILogger _logger;

    public MongoDbClient(string connectionString, string dbName, ILogger logger)
    {
        var mongoClient = new MongoClient(connectionString);
        _mongoDatabase = mongoClient.GetDatabase(dbName);
        _logger = logger;
        
        RegisterClassMap();
    }

    private static void RegisterClassMap()
    {
        try
        {
            BsonClassMap.RegisterClassMap<Category>(c => c.AutoMap());
        }
        catch (Exception)
        {
            // ignored
        }
    }

    public async Task<bool> CheckDbConnectionAsync()
    {
        _logger.LogInformationEx("Checking db connection...");

        try
        {
            var result = await _mongoDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}");

            if (result.Names.Contains("ok"))
            {
                _logger.LogInformationEx("Connection is established");
                return true;
            }
            
            _logger.LogInformationEx("Connection has not been established");   
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogInformationEx("Connection has not been established with error", ex);   
            return false;
        }
    }

    public async Task SaveArticlesAsync(Category category)
    {
        _logger.LogInformationEx("Start saving to db...");
        
        var catCollection = _mongoDatabase.GetCollection<Category>("category");

        var existingCategory = await catCollection
            .Find(c => c.Name == category.Name)
            .FirstOrDefaultAsync();

        if (existingCategory != null)
        {
            _logger.LogInformationEx("Adding new articles into exist category...");
            var newArticles = category.Articles
                .Where(article => existingCategory.Articles.All(existingArticle => existingArticle.ArticleUrl != article.ArticleUrl))
                .ToList();

            if (newArticles.Any())
            {
                var update = Builders<Category>.Update
                    .AddToSetEach(c => c.Articles, newArticles);

                await catCollection.UpdateOneAsync(
                    c => c.Id == existingCategory.Id,
                    update);
            }
            
            _logger.LogInformationEx($"Successfully updated category: {category.Name} with '{newArticles.Count}' articles to db!");
        }
        else
        {
            _logger.LogInformationEx("Creating new category...");
            await catCollection.InsertOneAsync(category);
            _logger.LogInformationEx($"Successfully created category: {category.Name} with '{category.Articles.Count}' articles to db!");
        }
        
    }
}