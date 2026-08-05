using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.Tests.UiTests;

public class DashboardTests : AdminTest
{
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDashboard()
    {
        var dashboardPage = new DashboardPage(Page);

        Assert.That(await dashboardPage.IsLoadedAsync(), Is.True);
    }
    
    [Test]
    [Category("UI")]
    public async Task AdminCanClickToOpenOrangeCom()
    {
        var dashboardPage = new DashboardPage(Page);

        var orangeComPage = await dashboardPage.OpenOrangeComAsync();

        try
        {
            await Expect(orangeComPage.GetByRole(AriaRole.Heading, new() { Name = "Global" })).ToBeVisibleAsync();
            Assert.That(new Uri(orangeComPage.Url).Host, Does.EndWith("orangehrm.com"));
        }
        finally
        {
            await orangeComPage.CloseAsync();
        }

        Assert.That(await dashboardPage.IsLoadedAsync(), Is.True);
    }
}
