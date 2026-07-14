using System.Threading.Tasks;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.Base;
[AllureNUnit]
public abstract class UiBaseTest
{
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;
    protected AppConfiguration Configuration = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        Configuration = ConfigurationReader.Current;

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        IBrowserType browserType = Configuration.Browser.ToLowerInvariant() switch
        {
            "firefox" => Playwright.Firefox,
            "webkit" => Playwright.Webkit,
            _ => Playwright.Chromium
        };

        Browser = await browserType.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = Configuration.Headless
            });

        Context = await Browser.NewContextAsync(CreateContextOptions());

        Page = await Context.NewPageAsync();

        Page.SetDefaultTimeout(Configuration.DefaultTimeoutMilliseconds);

        await Page.GotoAsync(Configuration.BaseUrl);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await Context.CloseAsync();
        await Browser.CloseAsync();
        Playwright.Dispose();
    }

    protected virtual BrowserNewContextOptions CreateContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = Configuration.BaseUrl
        };
    }
}
