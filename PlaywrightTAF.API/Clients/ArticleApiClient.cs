using PlaywrightTAF.API.Endpoints;
using PlaywrightTAF.API.ResponseModels;
using PlaywrightTAF.API.RequestModels;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public class ArticleApiClient : ApiClient
{
    public async Task<ArticleResponse?> CreateArticleAsync(ArticleRequest articleRequest, string token)
    {
        var response = await ExecuteAsync<ArticleResponse>(CreateArticleRequest(articleRequest, token));

        return response.Data;
    }

    public Task<RestResponse<ArticleResponse>> SendCreateArticleAsync(ArticleRequest articleRequest, string token)
    {
        return ExecuteRawAsync<ArticleResponse>(CreateArticleRequest(articleRequest, token));
    }

    public async Task<ArticleResponse?> GetArticleAsync(string slug)
    {
        var response = await ExecuteAsync<ArticleResponse>(CreateGetArticleRequest(slug));

        return response.Data;
    }

    public Task<RestResponse<ArticleResponse>> SendGetArticleAsync(string slug)
    {
        return ExecuteRawAsync<ArticleResponse>(CreateGetArticleRequest(slug));
    }

    public async Task<ArticleResponse?> UpdateArticleAsync(string slug, ArticleRequest articleRequest, string token)
    {
        var response = await ExecuteAsync<ArticleResponse>(CreateUpdateArticleRequest(slug, articleRequest, token));

        return response.Data;
    }

    public Task<RestResponse<ArticleResponse>> SendUpdateArticleAsync(string slug, ArticleRequest articleRequest, string token)
    {
        return ExecuteRawAsync<ArticleResponse>(CreateUpdateArticleRequest(slug, articleRequest, token));
    }

    public async Task<ArticleResponse?> FavoriteArticleAsync(string slug, string token)
    {
        var response = await ExecuteAsync<ArticleResponse>(CreateFavoriteArticleRequest(slug, token));

        return response.Data;
    }

    public Task<RestResponse<ArticleResponse>> SendFavoriteArticleAsync(string slug, string token)
    {
        return ExecuteRawAsync<ArticleResponse>(CreateFavoriteArticleRequest(slug, token));
    }

    public async Task<ArticleResponse?> UnfavoriteArticleAsync(string slug, string token)
    {
        var response = await ExecuteAsync<ArticleResponse>(CreateUnfavoriteArticleRequest(slug, token));

        return response.Data;
    }

    public Task<RestResponse<ArticleResponse>> SendUnfavoriteArticleAsync(string slug, string token)
    {
        return ExecuteRawAsync<ArticleResponse>(CreateUnfavoriteArticleRequest(slug, token));
    }

    public Task DeleteArticleAsync(string slug, string token)
    {
        return ExecuteAsync(CreateDeleteArticleRequest(slug, token));
    }

    public Task<RestResponse> SendDeleteArticleAsync(string slug, string token)
    {
        return ExecuteRawAsync(CreateDeleteArticleRequest(slug, token));
    }

    private static RestRequest CreateArticleRequest(ArticleRequest articleRequest, string token)
    {
        var request = CreateAuthorizedRequest(ApiEndpoints.Articles, Method.Post, token);

        request.AddJsonBody(articleRequest);

        return request;
    }

    private static RestRequest CreateGetArticleRequest(string slug)
    {
        return new RestRequest($"{ApiEndpoints.Articles}/{slug}", Method.Get);
    }

    private static RestRequest CreateUpdateArticleRequest(string slug, ArticleRequest articleRequest, string token)
    {
        var request = CreateAuthorizedRequest($"{ApiEndpoints.Articles}/{slug}", Method.Put, token);

        request.AddJsonBody(articleRequest);

        return request;
    }

    private static RestRequest CreateFavoriteArticleRequest(string slug, string token)
    {
        return CreateAuthorizedRequest($"{ApiEndpoints.Articles}/{slug}/favorite", Method.Post, token);
    }

    private static RestRequest CreateUnfavoriteArticleRequest(string slug, string token)
    {
        return CreateAuthorizedRequest($"{ApiEndpoints.Articles}/{slug}/favorite", Method.Delete, token);
    }

    private static RestRequest CreateDeleteArticleRequest(string slug, string token)
    {
        return CreateAuthorizedRequest($"{ApiEndpoints.Articles}/{slug}", Method.Delete, token);
    }

    private static RestRequest CreateAuthorizedRequest(string resource, Method method, string token)
    {
        var request = new RestRequest(resource, method);

        request.AddHeader("Authorization", $"Token {token}");

        return request;
    }
}
