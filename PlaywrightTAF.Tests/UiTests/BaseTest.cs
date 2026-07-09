using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.UiTests;

public abstract class BaseTest
{
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        Browser = await Playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 500
            });

        Context = await Browser.NewContextAsync();

        Page = await Context.NewPageAsync();

        await Page.GotoAsync(ConfigurationReader.Current.BaseUrl);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await Context.CloseAsync();
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}