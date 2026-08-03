using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages.AdminPages.Base;

public class BasePageAdmin : BasePage
{
    private const string AdminUsersPath = "/web/index.php/admin/viewSystemUsers";
    private const string CorporateBrandingPath = "/web/index.php/admin/addTheme";

    private ILocator AdminMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Admin" });
    private ILocator CorporateBrandingMenuLink => Page.GetByRole(AriaRole.Link, new() { Name = "Corporate Branding" });
    private ILocator SystemUsersText => Page.Locator("text=System Users");
    private ILocator CorporateBrandingHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Corporate Branding" });

    protected override string PageUrl => BuildUrl(ConfigurationReader.Current.BaseUrl, AdminUsersPath);

    public override async Task<bool> IsLoadedAsync()
    {
        return await SystemUsersText.IsVisibleAsync();
    }

    public BasePageAdmin(IPage page) : base(page)
    {
    }
    
    public async Task OpenAdminPageAsync()
    {
        await AdminMenuLink.ClickAsync();
        await Page.WaitForURLAsync($"**{AdminUsersPath}");
        await WaitUntilVisibleAsync(SystemUsersText);
    }

    public async Task ClickToOpenCorporateBrandingAsync()
    {
        await CorporateBrandingMenuLink.ClickAsync();
        await Page.WaitForURLAsync($"**{CorporateBrandingPath}");
        await WaitUntilVisibleAsync(CorporateBrandingHeading);
    }
}
