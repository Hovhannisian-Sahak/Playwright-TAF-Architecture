using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class DashboardTests : AdminTest
{
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDashboard()
    {
        var dashboardPage = new DashboardPage(Page);

        await dashboardPage.OpenAsync();

        Assert.That(await dashboardPage.IsLoadedAsync(), Is.True);
    }
}
