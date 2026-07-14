using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.Tests;

public class UserPermissionTests : UserTest
{
    [Test]
    public async Task User_Should_Not_Access_Admin_Page()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenLoginPageAsync();

        await loginPage.LoginAsync(ConfigurationReader.Current.User.Username, ConfigurationReader.Current.User.Password);

        var mainPage = new MainPage(Page);
        await mainPage.IsLoadedAsync();

        var currentUrl = Page.Url;

        Assert.That(currentUrl, Does.Not.Contain("/admin"));
    }
}
