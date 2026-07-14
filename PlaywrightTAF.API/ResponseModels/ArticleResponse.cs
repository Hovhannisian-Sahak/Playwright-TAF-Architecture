namespace PlaywrightTAF.Core.Models;

public class ArticleResponse
{
    public ArticleData article { get; set; } = new();
}

public class ArticleData
{
    public string slug { get; set; } = string.Empty;

    public string title { get; set; } = string.Empty;

    public string description { get; set; } = string.Empty;

    public string body { get; set; } = string.Empty;

    public List<string> tagList { get; set; } = [];

    public bool favorited { get; set; }

    public int favoritesCount { get; set; }

    public DateTime createdAt { get; set; }

    public DateTime updatedAt { get; set; }

    public AuthorData author { get; set; } = new();
}

public class AuthorData
{
    public string username { get; set; } = string.Empty;

    public string bio { get; set; } = string.Empty;

    public string image { get; set; } = string.Empty;

    public bool following { get; set; }
}
