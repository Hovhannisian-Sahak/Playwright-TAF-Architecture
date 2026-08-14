using Microsoft.Playwright;
using PlaywrightTAF.UI.Components;
using PlaywrightTAF.UI.Pages.UserManagementPages.Base;

namespace PlaywrightTAF.UI.Pages.UserManagementPages;

public class EditUserPage : UserManagementPageBase
{
    private readonly ToastMessage _toastMessage;

    public EditUserPage(IPage page, ToastMessage toastMessage) : base(page)
    {
        _toastMessage = toastMessage;
    }

    private ILocator FirstEditButton => FirstTableRowActionButton(1);

    private ILocator ChangePasswordCheckbox => Page.Locator(".oxd-checkbox-input");
    private ILocator PasswordRow => Page.Locator(".user-password-row");
    private ILocator SaveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Save" });

    public async Task EditFirstSearchResultAsync(string changedUsername, string changedPassword)
    {
        await FirstEditButton.ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await ChangePasswordCheckbox.ClickAsync();
        await PasswordRow.WaitForAsync();

        await PasswordInput.FillAsync(changedPassword);
        await ConfirmPasswordInput.FillAsync(changedPassword);
        await UsernameInput.FillAsync(changedUsername);

        await SaveButton.ClickAsync();

        await _toastMessage.WaitForUpdatedAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
