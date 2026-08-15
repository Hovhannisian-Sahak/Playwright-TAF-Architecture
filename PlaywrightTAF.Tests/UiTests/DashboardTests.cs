using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class DashboardTests : AdminTest
{
    private DashboardPage DashboardPage => PageObject<DashboardPage>();

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDashboard()
    {
        Assert.That(await DashboardPage.IsLoadedAsync(), Is.True);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanClickToOpenOrangeCom()
    {
        var orangeComPage = await DashboardPage.OpenOrangeComAsync();

        await orangeComPage.CloseAsync();

        Assert.That(await DashboardPage.IsLoadedAsync(), Is.True);
    }
}
