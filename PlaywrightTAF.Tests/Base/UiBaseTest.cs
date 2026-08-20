using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.NUnit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Core.Logging;
using PlaywrightTAF.Tests.DependencyInjection;
using PlaywrightTAF.Tests.Infrastructure;
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
    protected IServiceProvider Services = null!;

    protected virtual bool ShouldLoginThroughUi { get; } = true;

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
    }

    [SetUp]
    public async Task SetUpAsync()
    {
        Logger.Information("Starting UI test {TestName}", TestContext.CurrentContext.Test.FullName);
        Context = await Browser.NewContextAsync(CreateContextOptions());
        Page = await CreatePageAsync(Context);
        Services = CreateServices(Page);

        await NavigateToInitialUrlAsync();

        if (ShouldLoginThroughUi)
        {
            await LoginThroughUiAsync();
        }
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

        try
        {
            await CleanupTestDataAsync();
        }
        finally
        {
            await DisposeTestResourcesAsync();
        }
    }

    protected virtual Task CleanupTestDataAsync()
    {
        return Task.CompletedTask;
    }

    private async Task CaptureFailureScreenshotAsync()
    {
        Directory.CreateDirectory("screenshots");

        string testName = TestContext.CurrentContext.Test.FullName ?? TestContext.CurrentContext.Test.Name;
        string screenshotFileName = $"{SanitizeFileName(testName)}-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}.png";
        string screenshotPath = Path.Combine("screenshots", screenshotFileName);

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
        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }

    private static string SanitizeFileName(string value)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        var sanitizedChars = value.Select(character => invalidChars.Contains(character) ? '_' : character);

        return string.Concat(sanitizedChars);
    }

    private async Task DisposeTestResourcesAsync()
    {
        if (Services is IDisposable disposableServices)
        {
            disposableServices.Dispose();
        }

        Services = null!;

        if (Context is not null)
        {
            await Context.CloseAsync();
        }

        Context = null!;
        Page = null!;
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
        return PlaywrightBrowserFactory.GetBrowserType(Playwright, Configuration.Browser);
    }

    protected virtual Task<IBrowser> LaunchBrowserAsync()
    {
        return PlaywrightBrowserFactory.LaunchBrowserAsync(Playwright, Configuration);
    }

    protected virtual async Task<IPage> CreatePageAsync(IBrowserContext context)
    {
        var page = await context.NewPageAsync();
        page.SetDefaultTimeout(Configuration.DefaultTimeoutMilliseconds);

        return page;
    }

    protected virtual IServiceProvider CreateServices(IPage page)
    {
        return new ServiceCollection()
            .AddSingleton(page)
            .AddUiPageObjects()
            .BuildServiceProvider();
    }

    protected TPage PageObject<TPage>()
        where TPage : notnull
    {
        return Services.GetRequiredService<TPage>();
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
        var loginPage = PageObject<LoginPage>();
        await loginPage.OpenLoginPageAsync();
        await loginPage.LoginAsync(UiCredentials.Username, UiCredentials.Password);

    }

}
