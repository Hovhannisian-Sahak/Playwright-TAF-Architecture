using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.Authentication;

public static class AuthSetup
{
    public static async Task CreateAuthStateAsync(Credentials credentials, string storageStatePath)
    {
        AuthStatePaths.EnsureDirectoryExists();

        using var playwright = await Playwright.CreateAsync();

        var configuration = ConfigurationReader.Current;

        IBrowserType browserType = configuration.Browser.ToLowerInvariant() switch
        {
            "firefox" => playwright.Firefox,
            "webkit" => playwright.Webkit,
            _ => playwright.Chromium
        };

        var browser = await browserType.LaunchAsync(
            new()
            {
                Headless = configuration.Headless
            });

        var context = await browser.NewContextAsync(
            new()
            {
                BaseURL = configuration.BaseUrl
            });

        var page = await context.NewPageAsync();

        await page.GotoAsync(ConfigurationReader.Current.BaseUrl);

        var loginPage = new LoginPage(page);

        await loginPage.LoginAsync(credentials.Username, credentials.Password);

        await context.StorageStateAsync(new() { Path = storageStatePath });

        await browser.CloseAsync();
    }
}
