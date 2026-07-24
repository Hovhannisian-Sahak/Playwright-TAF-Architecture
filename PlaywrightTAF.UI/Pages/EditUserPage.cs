using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Pages;

public class EditUserPage : UserManagementPageBase
{
    public EditUserPage(IPage page) : base(page)
    {
    }

    public async Task EditFirstSearchResultAsync(string changedUsername, string changedPassword)
    {
        await Page.Locator(".oxd-table-cell-actions")
            .Locator("button")
            .Nth(1)
            .ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.Locator(".oxd-checkbox-input").ClickAsync();
        await Page.Locator(".user-password-row").WaitForAsync();

        await PasswordInput.FillAsync(changedPassword);
        await ConfirmPasswordInput.FillAsync(changedPassword);
        await UsernameInput.FillAsync(changedUsername);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Page.GetByText("Successfully Updated", new() { Exact = true }).WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
