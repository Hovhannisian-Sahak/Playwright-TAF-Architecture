using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Performance.Models;

namespace Performance.Clients;

public sealed class ConduitApiClient
{
    private readonly HttpClient _httpClient;

    public ConduitApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RegisteredApiUser> RegisterUserAsync(int virtualUser, CancellationToken cancellationToken)
    {
        var suffix = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{virtualUser}";
        var username = $"perf{suffix}";
        var email = $"{username}@mail.com";
        var payload = JsonSerializer.Serialize(new
        {
            user = new
            {
                username,
                email,
                password = "Password123"
            }
        });

        using var response = await _httpClient.PostAsync(
            ConduitApiEndpoints.RegisterUser,
            CreateJsonContent(payload),
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, content);

        using var document = JsonDocument.Parse(content);
        var user = document.RootElement.GetProperty("user");

        return new RegisteredApiUser(
            user.GetProperty("username").GetString() ?? username,
            user.GetProperty("token").GetString()
            ?? throw new InvalidOperationException("Register response did not include token."));
    }

    public async Task<string> CreateArticleAsync(string token, int virtualUser, CancellationToken cancellationToken)
    {
        var suffix = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{virtualUser}{Guid.NewGuid():N}"[..28];
        var payload = JsonSerializer.Serialize(new
        {
            article = new
            {
                title = $"Perf Article {suffix}",
                description = "Created by .NET performance test.",
                body = "Performance body.",
                tagList = new[] { "taf", "performance" }
            }
        });

        using var request = CreateAuthorizedRequest(HttpMethod.Post, ConduitApiEndpoints.Articles, token);
        request.Content = CreateJsonContent(payload);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, content);

        using var document = JsonDocument.Parse(content);
        return document.RootElement.GetProperty("article").GetProperty("slug").GetString()
               ?? throw new InvalidOperationException("Create article response did not include slug.");
    }

    public async Task DeleteArticleAsync(string token, string slug, CancellationToken cancellationToken)
    {
        using var request = CreateAuthorizedRequest(HttpMethod.Delete, $"{ConduitApiEndpoints.Articles}/{slug}", token);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        EnsureSuccess(response, content);
    }

    private static HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string path, string token)
    {
        var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Token", token);
        return request;
    }

    private static StringContent CreateJsonContent(string payload)
    {
        return new StringContent(payload, Encoding.UTF8, "application/json");
    }

    private static void EnsureSuccess(HttpResponseMessage response, string content)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw new InvalidOperationException($"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {content}");
    }
}
