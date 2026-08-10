using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public sealed class MainPageTests : UiBaseTest
{
    private LoginPage LoginPage => PageObject<LoginPage>();
    private MainPage MainPage => PageObject<MainPage>();

    [Test]
    [Category("UI")]
    public async Task MainPageShouldOpen()
    {
        await MainPage.OpenAsync();
        bool isLoaded = await MainPage.IsLoadedAsync();

        Assert.Multiple(() =>
        {
            Assert.That(MainPage.CurrentUrl, Does.StartWith(ConfigurationReader.Current.BaseUrl));
            Assert.That(isLoaded, Is.True);
        });
    }

    [Test]
    [Category("UI")]
    public async Task MainPageShouldLogout()
    {
        await MainPage.LogoutAsync();

        bool isLoaded = await LoginPage.IsLoadedAsync();

        Assert.Multiple(() =>
        {
            Assert.That(LoginPage.CurrentUrl, Does.Contain("/web/index.php/auth/login"));
            Assert.That(isLoaded, Is.True);
        });
    }
}
