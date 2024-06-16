using MongoDB.Bson;

namespace Binbi.Parser.DB.Models;

/// <summary>
/// Категория статьи
/// </summary>
public class Category
{
    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public ObjectId Id { get; set; }
    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Список статей
    /// </summary>
    public List<Article> Articles { get; set; } = null!;
}