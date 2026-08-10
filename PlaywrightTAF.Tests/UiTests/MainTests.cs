using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public sealed class MainPageTests : UiBaseTest
{
    private readonly LoginPage loginPage;
    private readonly MainPage mainPage;

    public MainPageTests()
    {
        loginPage = PageObject<LoginPage>();
        mainPage = PageObject<MainPage>();
    }

    [Test]
    [Category("UI")]
    public async Task MainPageShouldOpen()
    {
        await mainPage.OpenAsync();
        bool isLoaded = await mainPage.IsLoadedAsync();

        Assert.Multiple(() =>
        {
            Assert.That(mainPage.CurrentUrl, Does.StartWith(ConfigurationReader.Current.BaseUrl));
            Assert.That(isLoaded, Is.True);
        });
    }

    [Test]
    [Category("UI")]
    public async Task MainPageShouldLogout()
    {
        await mainPage.LogoutAsync();

        bool isLoaded = await loginPage.IsLoadedAsync();

        Assert.Multiple(() =>
        {
            Assert.That(loginPage.CurrentUrl, Does.Contain("/web/index.php/auth/login"));
            Assert.That(isLoaded, Is.True);
        });
    }
}
