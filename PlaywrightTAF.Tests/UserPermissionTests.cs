using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Authentication;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserPermissionTests : UserTest
{
    private const string EmployeeName = "Testing  qa";

    private DashboardPage DashboardPage => PageObject<DashboardPage>();

    [OneTimeSetUp]
    public override async Task OneTimeSetUpAsync()
    {
        await AuthSetup.EnsureUserExistsAsync(ConfigurationReader.Current.User, EmployeeName);

        if (File.Exists(AuthStatePaths.User))
        {
            File.Delete(AuthStatePaths.User);
        }

        await base.OneTimeSetUpAsync();
    }

    [Test]
    [Category("UI")]
    public async Task User_Should_Not_Access_Admin_Page()
    {
        Assert.That(await DashboardPage.IsLoadedAsync(), Is.True);

        var currentUrl = DashboardPage.CurrentUrl;

        Assert.That(currentUrl, Does.Not.Contain("/admin"));
    }
}
