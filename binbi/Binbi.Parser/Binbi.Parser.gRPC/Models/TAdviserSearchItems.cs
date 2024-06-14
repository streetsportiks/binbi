namespace Binbi.Parser.Models;

public class TAdviserSearchModel
{
    public List<TAdviserSearchItems>? Items { get; set; }
}

public class TAdviserSearchItems
{
    public string? Title { get; set; }
    public string? PublishDate { get; set; }
    public long PublishDateTimeStamp { get; set; }
    public string? ArticleUrl { get; set; }
}