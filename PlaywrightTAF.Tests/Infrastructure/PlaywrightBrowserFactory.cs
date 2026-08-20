using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.Infrastructure;

internal static class PlaywrightBrowserFactory
{
    public static IBrowserType GetBrowserType(IPlaywright playwright, string browser)
    {
        return browser.ToLowerInvariant() switch
        {
            "firefox" => playwright.Firefox,
            "webkit" => playwright.Webkit,
            _ => playwright.Chromium
        };
    }

    public static Task<IBrowser> LaunchBrowserAsync(
        IPlaywright playwright,
        AppConfiguration configuration,
        bool? headless = null)
    {
        return GetBrowserType(playwright, configuration.Browser)
            .LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless ?? configuration.Headless
            });
    }
}
