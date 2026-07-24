using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Pages;

public class EditUserPage : UserManagementPageBase
{
    public EditUserPage(IPage page) : base(page)
    {
    }

    private ILocator FirstEditButton => Page.Locator(".oxd-table-cell-actions")
        .Locator("button")
        .Nth(1);

    private ILocator ChangePasswordCheckbox => Page.Locator(".oxd-checkbox-input");
    private ILocator PasswordRow => Page.Locator(".user-password-row");
    private ILocator SaveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Save" });
    private ILocator SuccessfullyUpdatedText => Page.GetByText("Successfully Updated", new() { Exact = true });

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

        await SuccessfullyUpdatedText.WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
