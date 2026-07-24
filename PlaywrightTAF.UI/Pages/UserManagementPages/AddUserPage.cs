using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages;

public class AddUserPage : UserManagementPageBase
{
    public AddUserPage(IPage page) : base(page)
    {
    }

    private ILocator AddButton => Page.GetByRole(AriaRole.Button, new() { Name = " Add " });
    private ILocator AddUserHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Add User" });
    private ILocator Dropdowns => Page.Locator(".oxd-select-wrapper");
    private ILocator EmployeeNameInput => Page.Locator("input[placeholder='Type for hints...']");
    private ILocator EmployeeOptions => Page.Locator(".oxd-autocomplete-option");
    private ILocator SaveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Save" });
    private ILocator SuccessText => Page.GetByText("Success", new() { Exact = true });
    private ILocator Listbox => Page.GetByRole(AriaRole.Listbox);

    public async Task OpenAddUserFormAsync()
    {
        await OpenUserManagementAsync();
        await AddButton.ClickAsync();
        await Expect(AddUserHeading).ToBeVisibleAsync();
    }

    public override Task<bool> IsLoadedAsync()
    {
        return AddUserHeading.IsVisibleAsync();
    }

    public async Task CreateAdminUserAsync(string username, string employeeName, string password)
    {
        await SelectDropdownOptionAsync(0, "Admin");
        await SelectEmployeeAsync(employeeName);
        await SelectDropdownOptionAsync(1, "Enabled");

        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await ConfirmPasswordInput.FillAsync(password);
        await SaveButton.ClickAsync();

        await SuccessText.WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task SelectDropdownOptionAsync(int dropdownIndex, string option)
    {
        await Dropdowns.Nth(dropdownIndex).ClickAsync();

        await Listbox
            .GetByText(option, new() { Exact = true })
            .ClickAsync();
    }

    private async Task SelectEmployeeAsync(string employeeName)
    {
        await EmployeeNameInput.FillAsync(employeeName);

        await EmployeeOptions
            .Filter(new() { HasText = employeeName })
            .First
            .ClickAsync();
    }
}
