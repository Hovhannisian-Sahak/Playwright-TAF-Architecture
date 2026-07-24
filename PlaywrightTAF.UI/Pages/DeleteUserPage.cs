using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Pages;

public class DeleteUserPage : UserManagementPageBase
{
    public DeleteUserPage(IPage page) : base(page)
    {
    }

    public async Task DeleteFirstSearchResultAsync()
    {
        await Page.Locator(".oxd-table-cell-actions")
            .Locator("button")
            .Nth(0)
            .ClickAsync();

        await Page.Locator(".orangehrm-modal-footer")
            .Locator("button")
            .Nth(1)
            .ClickAsync();

        await Page.GetByText("Successfully Deleted", new() { Exact = true }).WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
