using System;
using System.IO;
using System.Threading.Tasks;
using Allure.NUnit;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Core.Logging;
using PlaywrightTAF.UI.Pages;
using Serilog;

namespace PlaywrightTAF.Tests.Base;

[AllureNUnit]
public abstract class UiBaseTest
{
    private static readonly ILogger Logger = LogProvider.ForContext<UiBaseTest>();

    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;
    protected AppConfiguration Configuration = null!;

    protected virtual bool ShouldLoginThroughUi => true;

    protected virtual Credentials UiCredentials => Configuration.Admin;

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUpAsync()
    {
        Configuration = ConfigurationReader.Current;
        Logger.Information(
            "Starting UI test fixture {FixtureName}. Browser={Browser}, Headless={Headless}, BaseUrl={BaseUrl}",
            TestContext.CurrentContext.Test.ClassName,
            Configuration.Browser,
            Configuration.Headless,
            Configuration.BaseUrl);

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
        Logger.Information("UI test page initialized at {CurrentUrl}", Page.Url);

        if (ShouldLoginThroughUi)
        {
            await LoginThroughUiAsync();
        }
    }

    [SetUp]
    public Task SetUpAsync()
    {
        Logger.Information("Starting UI test {TestName}", TestContext.CurrentContext.Test.FullName);
        return Task.CompletedTask;
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status
            == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            Directory.CreateDirectory("screenshots");
            string screenshotPath = $"screenshots/{TestContext.CurrentContext.Test.Name}.png";

            await Page.ScreenshotAsync(new()
            {
                Path = screenshotPath
            });

            Logger.Error(
                "UI test failed: {TestName}. Screenshot saved to {ScreenshotPath}",
                TestContext.CurrentContext.Test.FullName,
                screenshotPath);
        }
        else
        {
            Logger.Information(
                "UI test finished with status {Status}: {TestName}",
                TestContext.CurrentContext.Result.Outcome.Status,
                TestContext.CurrentContext.Test.FullName);
        }

    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDownAsync()
    {
        // try
        // {
        //     if (ShouldLoginThroughUi)
        //     {
        //         await LogoutThroughUiAsync();
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Logger.Warning(ex, "UI logout failed during one-time cleanup.");
        // }

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

    protected virtual async Task LoginThroughUiAsync()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.OpenLoginPageAsync();
        await loginPage.LoginAsync(UiCredentials.Username, UiCredentials.Password);
    }

    protected virtual async Task LogoutThroughUiAsync()
    {
        var mainPage = new MainPage(Page);

        if (!await mainPage.IsLoadedAsync())
        {
            await mainPage.OpenMainPageAsync();
        }

        await mainPage.LogoutAsync();
    }
}
