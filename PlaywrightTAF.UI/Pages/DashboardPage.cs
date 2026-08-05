using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

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
        return newPage;
    }
}
