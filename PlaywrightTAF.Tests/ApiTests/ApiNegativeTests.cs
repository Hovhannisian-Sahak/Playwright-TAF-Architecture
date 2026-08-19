using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.API.Clients;
using PlaywrightTAF.API.RequestModels;
using PlaywrightTAF.Tests.TestData;

namespace PlaywrightTAF.Tests.ApiTests;

public class ApiNegativeTests
{
    private readonly ArticleApiClient _articleClient = new();
    private readonly AuthApiClient _authClient = new();
    private readonly UserApiClient _userClient = new();

    [Test]
    [Category("API")]
    [Category("Negative")]
    public async Task Login_WithInvalidCredentials_ShouldReturnForbidden()
    {
        var request = new LoginRequest
        {
            user = new UserLogin
            {
                email = $"missing-{TestDataFactory.UniqueUsername("user")}@mail.com",
                password = "WrongPassword123"
            }
        };

        var response = await _authClient.SendLoginAsync(request);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            Assert.That(response.Content, Is.Not.Empty);
        });
    }

    [Test]
    [Category("API")]
    [Category("Negative")]
    public async Task GetCurrentUser_WithInvalidToken_ShouldReturnUnauthorized()
    {
        var response = await _userClient.SendGetCurrentUserAsync("invalid-token");

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.Content, Does.Contain("message"));
        });
    }

    [Test]
    [Category("API")]
    [Category("Negative")]
    public async Task GetArticle_WithUnknownSlug_ShouldReturnNotFound()
    {
        string unknownSlug = $"missing-{TestDataFactory.UniqueUsername("article")}";

        var response = await _articleClient.SendGetArticleAsync(unknownSlug);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    [Category("API")]
    [Category("Negative")]
    public async Task CreateArticle_WithoutValidToken_ShouldReturnUnauthorized()
    {
        var request = new ArticleRequest
        {
            article = new ArticleRequestData
            {
                title = TestDataFactory.UniqueUsername("Unauthorized Article"),
                description = "This request should not create an article.",
                body = "Missing a valid authentication token.",
                tagList = ["taf", "negative"]
            }
        };

        var response = await _articleClient.SendCreateArticleAsync(request, "invalid-token");

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccessful, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.Content, Does.Contain("message"));
        });
    }
}
