namespace PlaywrightTAF.Core.RequestModels;

public class ArticleRequest
{
    public ArticleRequestData article { get; set; } = new();
}

public class ArticleRequestData
{
    public string title { get; set; } = string.Empty;

    public string description { get; set; } = string.Empty;

    public string body { get; set; } = string.Empty;

    public List<string> tagList { get; set; } = [];
}
