using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.UI.Pages.Base;

public class PimConfigurationBasePage : BasePage
{
    public PimConfigurationBasePage(IPage page) : base(page)
    {
    }
    private ILocator DataImportButton => Page.GetByRole(AriaRole.Menuitem, new() { Name = "Data Import" });
    private ILocator ConfigurationButton => Page.GetByText("Configuration");
    private ILocator PimButton => Page.GetByRole(AriaRole.Link, new() { Name = "PIM" });
    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(true);
    }
    
    protected override string PageUrl => "url";

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
