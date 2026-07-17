using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

[TestFixture]
public class LoginTests : UiBaseTest
{
    protected override bool ShouldLoginThroughUi => false;

    [Test]
    [Category("UI")]
    public async Task LoginWithValidCredentials()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenLoginPageAsync();

        await loginPage.LoginAsync(ConfigurationReader.Current.Admin.Username, ConfigurationReader.Current.Admin.Password);

        var mainPage = new MainPage(Page);
        bool isLoaded = await mainPage.IsLoadedAsync();

        Assert.Multiple(() =>
        {
            Assert.That(mainPage.CurrentUrl, Does.StartWith(ConfigurationReader.Current.BaseUrl));
            Assert.That(isLoaded, Is.True);
        });
    }
}
