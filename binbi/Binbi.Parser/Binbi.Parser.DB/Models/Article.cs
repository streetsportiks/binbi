using MongoDB.Bson;

namespace Binbi.Parser.DB.Models;

/// <summary>
/// Класс статьи
/// </summary>
public class Article
{
    /// <summary>
    /// Идентификатор статьи
    /// </summary>
    public ObjectId Id { get; set; }
    /// <summary>
    /// Название статьи
    /// </summary>
    public string Title {get;set;} = null!;
    /// <summary>
    /// Описание статьи
    /// </summary>
    public string Description {get;set;} = null!;
    /// <summary>
    /// Дата публикации
    /// </summary>
    public DateTime PublishDate {get;set;}
    /// <summary>
    /// Дата публикации в миллисекундах
    /// </summary>
    public long PublishDateTimeStamp {get;set;}
    /// <summary>
    /// Ссылка на статью
    /// </summary>
    public string ArticleUrl {get;set;} = null!;
    /// <summary>
    /// Содержание статьи
    /// </summary>
    public string Data {get;set;} = null!;
}