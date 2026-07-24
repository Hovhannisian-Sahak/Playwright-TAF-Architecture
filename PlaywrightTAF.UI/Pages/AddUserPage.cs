using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages;

public class AddUserPage : UserManagementPageBase
{
    public AddUserPage(IPage page) : base(page)
    {
    }

    public async Task OpenAddUserFormAsync()
    {
        await OpenUserManagementAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = " Add " }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Add User" })).ToBeVisibleAsync();
    }

    public override Task<bool> IsLoadedAsync()
    {
        return Page.GetByRole(AriaRole.Heading, new() { Name = "Add User" }).IsVisibleAsync();
    }

    public async Task CreateAdminUserAsync(string username, string employeeName, string password)
    {
        await SelectDropdownOptionAsync(0, "Admin");
        await SelectEmployeeAsync(employeeName);
        await SelectDropdownOptionAsync(1, "Enabled");

        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await ConfirmPasswordInput.FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Page.GetByText("Success", new() { Exact = true }).WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task SelectDropdownOptionAsync(int dropdownIndex, string option)
    {
        await Page.Locator(".oxd-select-wrapper")
            .Nth(dropdownIndex)
            .ClickAsync();

        await Page.GetByRole(AriaRole.Listbox)
            .GetByText(option, new() { Exact = true })
            .ClickAsync();
    }

    private async Task SelectEmployeeAsync(string employeeName)
    {
        await Page.Locator("input[placeholder='Type for hints...']")
            .FillAsync(employeeName);

        await Page.Locator(".oxd-autocomplete-option")
            .Filter(new() { HasText = employeeName })
            .First
            .ClickAsync();
    }
}
