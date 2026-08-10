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
    private readonly DashboardPage dashboardPage;

    public DashboardTests()
    {
        dashboardPage = PageObject<DashboardPage>();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDashboard()
    {
        Assert.That(await dashboardPage.IsLoadedAsync(), Is.True);
    }
    
    [Test]
    [Category("UI")]
    public async Task AdminCanClickToOpenOrangeCom()
    {
        var orangeComPage = await dashboardPage.OpenOrangeComAsync();

        await orangeComPage.CloseAsync();

        Assert.That(await dashboardPage.IsLoadedAsync(), Is.True);
    }
}
