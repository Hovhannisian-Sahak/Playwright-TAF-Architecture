using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages;

public sealed class DashboardPage : BasePage
{
    private const string DashboardPath = "/web/index.php/dashboard/index";

    public DashboardPage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => new Uri(new Uri(ConfigurationReader.Current.BaseUrl), DashboardPath).ToString();

    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(CurrentUrl.Contains("dashboard", StringComparison.OrdinalIgnoreCase));
    }
}
