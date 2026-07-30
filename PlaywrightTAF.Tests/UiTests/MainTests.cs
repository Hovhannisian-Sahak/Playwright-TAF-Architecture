using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public sealed class MainPageTests : UiBaseTest
{
    [Test]
    [Category("UI")]
    public async Task MainPageShouldOpen()
    {
        var mainPage = new MainPage(Page);
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
        var mainPage = new MainPage(Page);
        await mainPage.LogoutAsync();
        bool isLoaded = await mainPage.IsLoadedAsync();

        Assert.Multiple(() =>
        {
            Assert.That(mainPage.CurrentUrl, Does.StartWith(ConfigurationReader.Current.BaseUrl));
            Assert.That(isLoaded, Is.True);
        });
    }
}
