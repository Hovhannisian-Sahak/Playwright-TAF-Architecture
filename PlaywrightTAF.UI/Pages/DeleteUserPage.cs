using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Pages;

public class DeleteUserPage : UserManagementPageBase
{
    public DeleteUserPage(IPage page) : base(page)
    {
    }

    private ILocator FirstDeleteButton => Page.Locator(".oxd-table-cell-actions")
        .Locator("button")
        .Nth(0);

    private ILocator ConfirmDeleteButton => Page.Locator(".orangehrm-modal-footer")
        .Locator("button")
        .Nth(1);

    private ILocator SuccessfullyDeletedText => Page.GetByText("Successfully Deleted", new() { Exact = true });

    public async Task DeleteFirstSearchResultAsync()
    {
        await FirstDeleteButton.ClickAsync();

        await ConfirmDeleteButton.ClickAsync();

        await SuccessfullyDeletedText.WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
