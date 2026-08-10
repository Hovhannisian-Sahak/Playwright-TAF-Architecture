using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserPermissionTests : UserTest
{
    private LoginPage LoginPage => PageObject<LoginPage>();
    private MainPage MainPage => PageObject<MainPage>();

    [Test]
    public async Task User_Should_Not_Access_Admin_Page()
    {
        await LoginPage.OpenLoginPageAsync();

        await LoginPage.LoginAsync(ConfigurationReader.Current.User.Username, ConfigurationReader.Current.User.Password);

        Assert.That(await MainPage.IsLoadedAsync(), Is.True);

        var currentUrl = MainPage.CurrentUrl;

        Assert.That(currentUrl, Does.Not.Contain("/admin"));
    }
}
