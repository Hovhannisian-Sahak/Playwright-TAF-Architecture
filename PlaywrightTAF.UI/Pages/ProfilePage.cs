using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages;

public sealed class ProfilePage : BasePage
{
    private const string ProfilePath = "/profile";

    public ProfilePage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => new Uri(new Uri(ConfigurationReader.Current.BaseUrl), ProfilePath).ToString();

    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(CurrentUrl.Contains("profile", StringComparison.OrdinalIgnoreCase));
    }
}
