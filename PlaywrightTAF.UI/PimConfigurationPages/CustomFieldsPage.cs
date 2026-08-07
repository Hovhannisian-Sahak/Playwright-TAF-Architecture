using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Pages.Base;

namespace PlaywrightTAF.UI.Pages;

public class CustomFieldsPage : PimConfigurationBasePage
{
    public CustomFieldsPage(IPage page) : base(page)
    {
    }
    
    protected override string PageUrl => BuildUrl(ConfigurationReader.Current.BaseUrl, "/web/index.php/pim/listCustomFields");
    
    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(CurrentUrl.Contains("listCustomFields", StringComparison.OrdinalIgnoreCase));
    }
}