using PlaywrightTAF.API.Clients;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Models;
using PlaywrightTAF.Core.RequestModels;

namespace PlaywrightTAF.API.Services;

public class ArticleService
{
    private readonly ArticleApiClient _articleClient;
    private readonly TokenProvider _tokenProvider;

    public ArticleService(ArticleApiClient articleClient, TokenProvider tokenProvider)
    {
        _articleClient = articleClient;
        _tokenProvider = tokenProvider;
    }

    public async Task<ArticleData> CreateArticle(string title, string description, string body, List<string>? tags = null)
    {
        var response = await _articleClient.CreateArticleAsync(
            CreateRequest(title, description, body, tags),
            _tokenProvider.GetToken());

        return response!.article;
    }

    public async Task<ArticleData> GetArticle(string slug)
    {
        var response = await _articleClient.GetArticleAsync(slug);

        return response!.article;
    }

    public async Task<ArticleData> UpdateArticle(string slug, string title, string description, string body, List<string>? tags = null)
    {
        var response = await _articleClient.UpdateArticleAsync(
            slug,
            CreateRequest(title, description, body, tags),
            _tokenProvider.GetToken());

        return response!.article;
    }

    public Task DeleteArticle(string slug)
    {
        return _articleClient.DeleteArticleAsync(slug, _tokenProvider.GetToken());
    }

    private static ArticleRequest CreateRequest(string title, string description, string body, List<string>? tags)
    {
        return new ArticleRequest
        {
            article = new ArticleRequestData
            {
                title = title,
                description = description,
                body = body,
                tagList = tags ?? []
            }
        };
    }
}
