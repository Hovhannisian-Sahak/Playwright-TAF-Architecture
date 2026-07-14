using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public sealed class MainPageTests : UiBaseTest
{
    [Test]
    public async Task MainPageShouldOpen()
    {
        var mainPage = new MainPage(Page);

        bool isLoaded = await mainPage.IsLoadedAsync();
        // string bodyText = await mainPage.GetBodyTextAsync();

        Assert.Multiple(() =>
        {
            Assert.That(mainPage.CurrentUrl, Does.StartWith(ConfigurationReader.Current.BaseUrl));
            Assert.That(isLoaded, Is.True);

            // Assert.That(bodyText, Is.Not.Empty);
        });
    }
}
