using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons;
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

    protected virtual bool ShouldLoginThroughUi { get; } = true;

    protected virtual bool ShouldLogoutThroughUi => ShouldLoginThroughUi;

    protected virtual Credentials UiCredentials => Configuration.Admin;

    protected virtual string InitialUrl => Configuration.BaseUrl;

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

        Browser = await LaunchBrowserAsync();
        Context = await Browser.NewContextAsync(CreateContextOptions());
        Page = await CreatePageAsync(Context);

        await NavigateToInitialUrlAsync();

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
            await CaptureFailureScreenshotAsync();
        }
        else
        {
            Logger.Information(
                "UI test finished with status {Status}: {TestName}",
                TestContext.CurrentContext.Result.Outcome.Status,
                TestContext.CurrentContext.Test.FullName);
        }

    }

    private async Task CaptureFailureScreenshotAsync()
    {
        Directory.CreateDirectory("screenshots");

        string testName = TestContext.CurrentContext.Test.Name;
        string screenshotPath = Path.Combine("screenshots", $"{testName}.png");

        try
        {
            byte[] screenshot = await Page.ScreenshotAsync(new()
            {
                Path = screenshotPath,
                FullPage = true
            });

            AllureApi.AddAttachment(
                $"{testName} failure screenshot",
                "image/png",
                screenshot,
                ".png");

            Logger.Error(
                "UI test failed: {TestName}. Screenshot saved to {ScreenshotPath} and attached to Allure.",
                TestContext.CurrentContext.Test.FullName,
                screenshotPath);
        }
        catch (Exception ex)
        {
            Logger.Error(
                ex,
                "UI test failed: {TestName}. Could not capture or attach screenshot.",
                TestContext.CurrentContext.Test.FullName);
        }
    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDownAsync()
    {
        if (Context is not null)
        {
            await Context.CloseAsync();
        }

        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }
        
        Playwright?.Dispose();
    }

    protected virtual BrowserNewContextOptions CreateContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = Configuration.BaseUrl,
            AcceptDownloads = true
        };
    }

    protected virtual IBrowserType GetBrowserType()
    {
        return Configuration.Browser.ToLowerInvariant() switch
        {
            "firefox" => Playwright.Firefox,
            "webkit" => Playwright.Webkit,
            _ => Playwright.Chromium
        };
    }

    protected virtual Task<IBrowser> LaunchBrowserAsync()
    {
        return GetBrowserType().LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Configuration.Headless
        });
    }

    protected virtual async Task<IPage> CreatePageAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(Configuration.DefaultTimeoutMilliseconds);

        return page;
    }

    protected virtual async Task NavigateToInitialUrlAsync()
    {
        await Page.GotoAsync(
            InitialUrl,
            new()
            {
                WaitUntil = WaitUntilState.Commit,
                Timeout = Configuration.DefaultTimeoutMilliseconds * 2
            });

        Logger.Information("UI test page initialized at {CurrentUrl}", Page.Url);
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
