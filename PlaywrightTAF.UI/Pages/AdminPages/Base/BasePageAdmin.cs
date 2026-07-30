using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages.AdminPages.Base;

public class BasePageAdmin : BasePage
{
    private const string AdminUsersPath = "/web/index.php/admin/viewSystemUsers";
    private const string CorporateBrandingPath = "/web/index.php/admin/addTheme";

    private ILocator AdminMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Admin" });
    private ILocator CorporateBrandingMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Corporate Branding" });
    protected override string PageUrl => new Uri(new Uri(ConfigurationReader.Current.BaseUrl), AdminUsersPath).ToString();
    public override async Task<bool> IsLoadedAsync()
    {
        return await Page.Locator("text=System Users").IsVisibleAsync();
    }
    public BasePageAdmin(IPage page) : base(page)
    {
    }
    
    public async Task OpenAdminPageAsync()
    {
        await AdminMenuLink.ClickAsync();
        await Page.WaitForURLAsync($"**{AdminUsersPath}");
        await Page.Locator("text=System Users").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task ClickToOpenCorporateBrandingAsync()
    {
        await CorporateBrandingMenuLink.ClickAsync();
        await Page.WaitForURLAsync($"**{CorporateBrandingPath}");
        await Page
            .GetByRole(AriaRole.Heading, new() { Name = "Corporate Branding" })
            .WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }
}
