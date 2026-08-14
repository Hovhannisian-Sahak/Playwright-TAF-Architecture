using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.PimConfigurationPages.Base;

namespace PlaywrightTAF.UI.PimConfigurationPages;

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
