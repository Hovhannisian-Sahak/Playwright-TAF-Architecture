using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.UI.PimConfigurationPages.Base;

public class PimConfigurationBasePage : BasePage
{
    public PimConfigurationBasePage(IPage page) : base(page)
    {
    }

    private ILocator DataImportButton => Page.GetByRole(AriaRole.Menuitem, new() { Name = "Data Import" });
    private ILocator ConfigurationButton => Page.GetByText("Configuration");
    private ILocator PimButton => Page.GetByRole(AriaRole.Link, new() { Name = "PIM" });

    protected override string PageUrl => BuildUrl(ConfigurationReader.Current.BaseUrl, "/web/index.php/pim/viewEmployeeList");

    public override async Task<bool> IsLoadedAsync()
    {
        return CurrentUrl.Contains("/pim/", StringComparison.OrdinalIgnoreCase)
               && await ConfigurationButton.IsVisibleAsync();
    }

    public async Task WaitAndClickPimButton()
    {
        await PimButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await PimButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task WaitAndClickConfigurationButton()
    {
        await ConfigurationButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await ConfigurationButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task WaitAndClickDataImportButton()
    {
        await DataImportButton.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await DataImportButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
