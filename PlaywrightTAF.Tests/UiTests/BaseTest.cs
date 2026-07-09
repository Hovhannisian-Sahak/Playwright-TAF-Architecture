using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.UiTests;

public abstract class BaseTest : PageTest
{
    [SetUp]
    public async Task SetUpAsync()
    {
        await Page.GotoAsync(ConfigurationReader.Current.BaseUrl);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            await Page.ScreenshotAsync(new()
            {
                Path = $"TestResults/screenshots/{TestContext.CurrentContext.Test.Name}.png",
                FullPage = true
            });
        }
    }
}