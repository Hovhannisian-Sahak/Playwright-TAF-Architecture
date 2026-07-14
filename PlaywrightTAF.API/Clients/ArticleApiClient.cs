using PlaywrightTAF.API.Endpoints;
using PlaywrightTAF.Core.Models;
using PlaywrightTAF.Core.RequestModels;
using RestSharp;

namespace PlaywrightTAF.API.Clients;

public class ArticleApiClient : ApiClient
{
    public async Task<ArticleResponse?> CreateArticleAsync(ArticleRequest articleRequest, string token)
    {
        var request = CreateAuthorizedRequest(ApiEndpoints.Articles, Method.Post, token);

        request.AddJsonBody(articleRequest);

        var response = await ExecuteAsync<ArticleResponse>(request);

        return response.Data;
    }

    public async Task<ArticleResponse?> GetArticleAsync(string slug)
    {
        var request = new RestRequest($"{ApiEndpoints.Articles}/{slug}", Method.Get);

        var response = await ExecuteAsync<ArticleResponse>(request);

        return response.Data;
    }

    public async Task<ArticleResponse?> UpdateArticleAsync(string slug, ArticleRequest articleRequest, string token)
    {
        var request = CreateAuthorizedRequest($"{ApiEndpoints.Articles}/{slug}", Method.Put, token);

        request.AddJsonBody(articleRequest);

        var response = await ExecuteAsync<ArticleResponse>(request);

        return response.Data;
    }

    public Task DeleteArticleAsync(string slug, string token)
    {
        var request = CreateAuthorizedRequest($"{ApiEndpoints.Articles}/{slug}", Method.Delete, token);

        return ExecuteAsync(request);
    }

    private static RestRequest CreateAuthorizedRequest(string resource, Method method, string token)
    {
        var request = new RestRequest(resource, method);

        request.AddHeader("Authorization", $"Token {token}");

        return request;
    }
}
