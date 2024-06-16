using Binbi.Parser.DB.Models;
using Microsoft.Extensions.Logging;
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

    public async Task SaveArticlesAsync(Category category)
    {
        _logger.LogInformation("Start saving to db...");
        
        var catCollection = _mongoDatabase.GetCollection<Category>("category");

        var existingCategory = await catCollection
            .Find(c => c.Name == category.Name)
            .FirstOrDefaultAsync();

        if (existingCategory != null)
        {
            _logger.LogInformation("Adding new articles into exist category...");
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
            
            _logger.LogInformation($"Successfully updated category: {category.Name} with '{newArticles.Count}' articles to db!");
        }
        else
        {
            _logger.LogInformation("Creating new category...");
            await catCollection.InsertOneAsync(category);
            _logger.LogInformation($"Successfully created category: {category.Name} with '{category.Articles.Count}' articles to db!");
        }
        
    }
}