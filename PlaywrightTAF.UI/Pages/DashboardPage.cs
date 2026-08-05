using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages;

public sealed class DashboardPage : BasePage
{
    private const string DashboardPath = "/web/index.php/dashboard/index";
    private ILocator OrangeComLink => Page.GetByRole(AriaRole.Link, new() { Name = "OrangeHRM, Inc" });

    public DashboardPage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => new Uri(new Uri(ConfigurationReader.Current.BaseUrl), DashboardPath).ToString();

    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(CurrentUrl.Contains("dashboard", StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IPage> OpenOrangeComAsync()
    {
        var newPageTask = Page.Context.WaitForPageAsync();

        await OrangeComLink.ClickAsync();

        var newPage = await newPageTask;

        await newPage.WaitForLoadStateAsync();
        await newPage.WaitForURLAsync("**orangehrm.com**");
        await Expect(newPage.GetByRole(AriaRole.Button, new() { Name = "Global" })).ToBeVisibleAsync();

        await CloseCookieBannerIfVisibleAsync(newPage);

        await newPage.GetByRole(AriaRole.Link, new() { Name = "Company" }).HoverAsync();
        var careersLink = newPage.Locator("#navbarNav").GetByRole(AriaRole.Link, new() { Name = "Careers" });
        await careersLink.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var careersUrlTask = newPage.WaitForURLAsync("**/company/careers");
        await careersLink.ClickAsync();
        await careersUrlTask;

        await newPage.GoBackAsync();
        await newPage.WaitForURLAsync("**orangehrm.com**");
        await Expect(newPage.GetByRole(AriaRole.Button, new() { Name = "Global" })).ToBeVisibleAsync();

        return newPage;
    }

    private static async Task CloseCookieBannerIfVisibleAsync(IPage page)
    {
        var closeCookieButton = page.Locator("#CybotCookiebotBannerCloseButtonE2E");

        try
        {
            await closeCookieButton.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
            await closeCookieButton.ClickAsync();
            await page.Locator("#CybotCookiebotDialog").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
        }
        catch (TimeoutException)
        {
        }
    }
}
