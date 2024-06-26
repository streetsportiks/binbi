using System.Text.Json.Serialization;

namespace Binbi.Parser.API.Models;

public class RbcSearchModel
{
    [JsonPropertyName("items")]
    public List<RbcSearchItems>? Items { get; set; }
}

public class RbcSearchItems
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("publish_date")]
    public string? PublishDate { get; set; }
    [JsonPropertyName("publish_date_t")]
    public int PublishDateTimeStamp { get; set; }
    [JsonPropertyName("fronturl")]
    public string? ArticleUrl { get; set; }
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}