using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.NUnit;
using NUnit.Framework;
using PlaywrightTAF.API.Clients;
using PlaywrightTAF.API.Services;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Logging;
using Serilog;

namespace PlaywrightTAF.Tests.Base;

[AllureNUnit]
public abstract class BaseApiTest
{
    private static readonly ILogger Logger = LogProvider.ForContext<BaseApiTest>();
    private readonly List<string> _articleSlugsToDelete = [];

    protected const string TestPassword = "Password123";

    protected AuthService AuthService = null!;
    protected ArticleService ArticleService = null!;
    protected string TestEmail = string.Empty;
    protected string TestUsername = string.Empty;
    protected TokenProvider TokenProvider = null!;
    protected UserService UserService = null!;

    [SetUp]
    public async Task BaseSetup()
    {
        Logger.Information("Starting API test setup for {TestName}", TestContext.CurrentContext.Test.FullName);

        AuthService = new AuthService(new AuthApiClient());
        TokenProvider = new TokenProvider();
        ArticleService = new ArticleService(new ArticleApiClient(), TokenProvider);
        UserService = new UserService(new UserApiClient(), TokenProvider);

        TestEmail = $"taf-{Guid.NewGuid():N}@mail.com";
        TestUsername = $"tafuser{Guid.NewGuid():N}"[..15];

        await AuthService.Register(TestUsername, TestEmail, TestPassword);
        string token = await AuthService.Login(TestEmail, TestPassword);

        TokenProvider.SetToken(token);
        Logger.Information("API test user prepared: {Username} / {Email}", TestUsername, TestEmail);
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        await DeleteTrackedArticlesAsync();
    }

    protected void TrackArticleForCleanup(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug) || _articleSlugsToDelete.Contains(slug))
        {
            return;
        }

        _articleSlugsToDelete.Add(slug);
    }

    protected void UntrackArticleFromCleanup(string slug)
    {
        _articleSlugsToDelete.Remove(slug);
    }

    protected async Task DeleteTrackedArticleAsync(string slug)
    {
        await ArticleService.DeleteArticle(slug);
        UntrackArticleFromCleanup(slug);
    }

    private async Task DeleteTrackedArticlesAsync()
    {
        for (int index = _articleSlugsToDelete.Count - 1; index >= 0; index--)
        {
            string slug = _articleSlugsToDelete[index];

            try
            {
                await ArticleService.DeleteArticle(slug);
                Logger.Information("Deleted tracked API article {Slug}", slug);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Could not delete tracked API article {Slug}", slug);
            }
        }

        _articleSlugsToDelete.Clear();
    }
}
