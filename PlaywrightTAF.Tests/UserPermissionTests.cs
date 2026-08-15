using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserPermissionTests : UserTest
{
    private DashboardPage DashboardPage => PageObject<DashboardPage>();

    [Test]
    [Category("UI")]
    public async Task User_Should_Not_Access_Admin_Page()
    {
        Assert.That(await DashboardPage.IsLoadedAsync(), Is.True);
        
        var currentUrl = DashboardPage.CurrentUrl;

        Assert.That(currentUrl, Does.Not.Contain("/admin"));
    }
}
