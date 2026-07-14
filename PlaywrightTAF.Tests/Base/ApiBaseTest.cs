using System;
using System.Threading.Tasks;
using Allure.NUnit;
using NUnit.Framework;
using PlaywrightTAF.API.Clients;
using PlaywrightTAF.API.Services;
using PlaywrightTAF.Core.Authentication;

namespace PlaywrightTAF.Tests.Base;
[AllureNUnit]
public abstract class BaseApiTest
{
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
        AuthService = new AuthService(new AuthApiClient());
        TokenProvider = new TokenProvider();
        ArticleService = new ArticleService(new ArticleApiClient(), TokenProvider);
        UserService = new UserService(new UserApiClient(), TokenProvider);

        TestEmail = $"taf-{Guid.NewGuid():N}@mail.com";
        TestUsername = $"tafuser{Guid.NewGuid():N}"[..15];

        await AuthService.Register(TestUsername, TestEmail, TestPassword);
        string token = await AuthService.Login(TestEmail, TestPassword);

        TokenProvider.SetToken(token);
    }
}
