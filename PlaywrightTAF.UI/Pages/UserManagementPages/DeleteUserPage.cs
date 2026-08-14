using Microsoft.Playwright;
using PlaywrightTAF.UI.Components;
using PlaywrightTAF.UI.Pages.UserManagementPages.Base;

namespace PlaywrightTAF.UI.Pages.UserManagementPages;

public class DeleteUserPage : UserManagementPageBase
{
    private readonly ToastMessage _toastMessage;

    public DeleteUserPage(IPage page, ToastMessage toastMessage) : base(page)
    {
        _toastMessage = toastMessage;
    }

    private ILocator FirstDeleteButton => FirstTableRowActionButton(0);

    private ILocator ConfirmDeleteButton => Page.Locator(".orangehrm-modal-footer")
        .Locator("button")
        .Nth(1);

    public async Task DeleteFirstSearchResultAsync()
    {
        await FirstDeleteButton.ClickAsync();

        await ConfirmDeleteButton.ClickAsync();

        await _toastMessage.WaitForDeletedAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
