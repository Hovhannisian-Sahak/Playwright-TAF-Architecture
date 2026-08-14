using Microsoft.Playwright;
using PlaywrightTAF.UI.Components;
using PlaywrightTAF.UI.Pages.UserManagementPages.Base;

namespace PlaywrightTAF.UI.Pages.UserManagementPages;

public class AddUserPage : UserManagementPageBase
{
    private readonly ToastMessage _toastMessage;

    public AddUserPage(IPage page, ToastMessage toastMessage) : base(page)
    {
        _toastMessage = toastMessage;
    }

    private ILocator AddButton => Page.GetByRole(AriaRole.Button, new() { Name = " Add " });
    private ILocator AddUserHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Add User" });
    private ILocator Dropdowns => Page.Locator(".oxd-select-wrapper");
    private ILocator EmployeeNameInput => Page.Locator("input[placeholder='Type for hints...']");
    private ILocator EmployeeOptions => Page.Locator(".oxd-autocomplete-option");
    private ILocator SaveButton => Page.GetByRole(AriaRole.Button, new() { Name = "Save" });

    public async Task OpenAddUserFormAsync()
    {
        await OpenUserManagementAsync();
        await AddButton.ClickAsync();
        await WaitUntilVisibleAsync(AddUserHeading);
    }

    public override Task<bool> IsLoadedAsync()
    {
        return AddUserHeading.IsVisibleAsync();
    }

    public async Task CreateAdminUserAsync(string username, string employeeName, string password)
    {
        await SelectDropdownOptionAsync(Dropdowns, 0, "Admin");
        await SelectEmployeeAsync(employeeName);
        await SelectDropdownOptionAsync(Dropdowns, 1, "Enabled");

        await FillAndExpectValueAsync(UsernameInput, username);
        await FillAndExpectValueAsync(PasswordInput, password);
        await FillAndExpectValueAsync(ConfirmPasswordInput, password);
        await SaveButton.ClickAsync();

        await _toastMessage.WaitForSuccessAsync();
        await WaitForPageLoadAsync();
    }

    private async Task SelectEmployeeAsync(string employeeName)
    {
        await FillAndExpectValueAsync(EmployeeNameInput, employeeName);

        await EmployeeOptions
            .Filter(new() { HasText = employeeName })
            .First
            .ClickAsync();
    }
}
