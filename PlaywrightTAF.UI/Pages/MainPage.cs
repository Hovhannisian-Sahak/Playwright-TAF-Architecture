using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages;

public sealed class MainPage : BasePage
{
    public MainPage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => ConfigurationReader.Current.BaseUrl;

    private ILocator Body => Page.Locator("body");

    public Task OpenMainPageAsync()
    {
        return OpenAsync();
    }

    public Task<bool> IsLoadedAsync()
    {
        return Body.IsVisibleAsync();
    }

    public Task<string> GetBodyTextAsync()
    {
        return Body.InnerTextAsync();
    }
}
