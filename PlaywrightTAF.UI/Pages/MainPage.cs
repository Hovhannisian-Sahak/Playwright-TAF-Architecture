using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages;

public sealed class MainPage : BasePage
{
    public MainPage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => ConfigurationReader.Current.BaseUrl;

    private ILocator Body => Page.Locator(".oxd-userdropdown-name");
    private ILocator UserDropdown => Page.Locator(".oxd-userdropdown-tab");
    private ILocator LogoutLink => Page.GetByRole(AriaRole.Link, new() { Name = "Logout" });

    public Task OpenMainPageAsync()
    {
        return OpenAsync();
    }

    public override Task<bool> IsLoadedAsync()
    {
        return Body.IsVisibleAsync();
    }

    public async Task LogoutAsync()
    {
        await UserDropdown.ClickAsync();
        await LogoutLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    // public Task<string> GetBodyTextAsync()
    // {
    //     return Body.InnerTextAsync();
    // }
}
