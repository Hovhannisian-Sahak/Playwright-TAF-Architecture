using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages;

public abstract class UserManagementPageBase : BasePage
{
    protected UserManagementPageBase(IPage page) : base(page)
    {
    }

    protected override string PageUrl => ConfigurationReader.Current.BaseUrl;

    protected ILocator SearchFilter => Page.Locator(".oxd-table-filter");
    protected ILocator TableBody => Page.Locator(".oxd-table-body");
    protected ILocator UsernameInput => Page
        .Locator(".oxd-input-group")
        .Filter(new() { HasText = "Username" })
        .Locator("input");

    protected ILocator PasswordInput => Page.Locator("input[type='password']").Nth(0);
    protected ILocator ConfirmPasswordInput => Page.Locator("input[type='password']").Nth(1);

    protected async Task OpenUserManagementAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Admin" }).ClickAsync();
        await Expect(SearchFilter).ToBeVisibleAsync();
    }

    public override Task<bool> IsLoadedAsync()
    {
        return SearchFilter.IsVisibleAsync();
    }

    public async Task SearchUserAsync(string username)
    {
        var searchUsername = SearchFilter
            .Locator(".oxd-input-group")
            .Filter(new() { HasText = "Username" })
            .Locator("input");

        await searchUsername.FillAsync(username);
        await Expect(searchUsername).ToHaveValueAsync(username);

        await SearchFilter
            .GetByRole(AriaRole.Button, new() { Name = "Search" })
            .ClickAsync();
    }

    public async Task ExpectUserExistsAsync(string username)
    {
        await Expect(TableBody).ToContainTextAsync(username);
        await Expect(Page.Locator("text=(1) Record Found")).ToBeVisibleAsync();
    }

    public async Task ExpectUserDoesNotExistAsync(string username)
    {
        var noRecordsFoundText = Page.Locator(".orangehrm-horizontal-padding")
            .GetByText("No Records Found", new() { Exact = true });

        await Expect(noRecordsFoundText).ToBeVisibleAsync();
        await Expect(TableBody).Not.ToContainTextAsync(username);
    }
}
