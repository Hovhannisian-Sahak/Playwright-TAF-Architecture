using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

namespace PlaywrightTAF.Tests.UiTests;

public class DashboardTests : AdminTest
{
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDashboard()
    {
        await Page.GotoAsync("/dashboard");

        Assert.That(Page.Url,
            Does.Contain("dashboard"));
    }
}
