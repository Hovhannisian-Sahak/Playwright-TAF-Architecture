using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserPermissionTests : UserTest
{
    private readonly LoginPage loginPage;
    private readonly MainPage mainPage;

    public UserPermissionTests()
    {
        loginPage = PageObject<LoginPage>();
        mainPage = PageObject<MainPage>();
    }

    [Test]
    public async Task User_Should_Not_Access_Admin_Page()
    {
        await loginPage.OpenLoginPageAsync();

        await loginPage.LoginAsync(ConfigurationReader.Current.User.Username, ConfigurationReader.Current.User.Password);

        Assert.That(await mainPage.IsLoadedAsync(), Is.True);

        var currentUrl = mainPage.CurrentUrl;

        Assert.That(currentUrl, Does.Not.Contain("/admin"));
    }
}
