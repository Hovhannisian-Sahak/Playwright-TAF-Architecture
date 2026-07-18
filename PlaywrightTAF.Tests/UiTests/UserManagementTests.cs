﻿using System.Threading.Tasks;
using Microsoft.Playwright;
using System;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using static Microsoft.Playwright.Assertions;
namespace PlaywrightTAF.Tests.UiTests;

public class UserManagementTests : UiBaseTest
{
    private const string EmployeeName = "Ranga  Akunuri";
    private const string UserPassword = "TestUser123!@#Aa";
    private const string ChangePassword = "TestUser123!@#Aab";
    [Test]
    [Category("UI")]
    public async Task AdminCanAddUser()
    {
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await OpenAddUserFormAsync();
        await CreateAdminUserAsync(newUsername);
        await SearchUserAsync(newUsername);

        await ExpectUserExistsAsync(newUsername);

        await DeleteFirstSearchResultAsync();
    }
    [Test]
    [Category("UI")]
    public async Task AdminCanDeleteUser()
    {
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await OpenAddUserFormAsync();
        await CreateAdminUserAsync(newUsername);
        await SearchUserAsync(newUsername);

        await ExpectUserExistsAsync(newUsername);

        await DeleteFirstSearchResultAsync();
        await SearchUserAsync(newUsername);

        await ExpectUserDoesNotExistAsync(newUsername);
    }
    [Test]
    [Category("UI")]
    public async Task AdminCanChangeUserNameAndPassword()
    {
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var changedUsername = $"ChangedAdminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await OpenAddUserFormAsync();
        await CreateAdminUserAsync(newUsername);
        await SearchUserAsync(newUsername);

        await ExpectUserExistsAsync(newUsername);

        await EditFirstSearchResultAsync(changedUsername);
        await SearchUserAsync(changedUsername);
        
        await ExpectUserExistsAsync(changedUsername);
        await DeleteFirstSearchResultAsync();
    }

    private ILocator SearchFilter => Page.Locator(".oxd-table-filter");
    private ILocator TableBody => Page.Locator(".oxd-table-body");
    private ILocator NoRecordsFoundText =>
        Page.Locator(".orangehrm-horizontal-padding")
            .GetByText("No Records Found", new() { Exact = true });

    private ILocator UsernameInput => Page
        .Locator(".oxd-input-group")
        .Filter(new() { HasText = "Username" })
        .Locator("input");

    private ILocator PasswordInput => Page.Locator("input[type='password']").Nth(0);
    private ILocator ConfirmPasswordInput => Page.Locator("input[type='password']").Nth(1);

    private async Task OpenAddUserFormAsync()
    {
        await Page.Locator(".oxd-main-menu li").First.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = " Add " }).ClickAsync();
    }

    private async Task CreateAdminUserAsync(string username)
    {
        await SelectDropdownOptionAsync(0, "Admin");
        await SelectEmployeeAsync(EmployeeName);
        await SelectDropdownOptionAsync(1, "Enabled");

        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(UserPassword);
        await ConfirmPasswordInput.FillAsync(UserPassword);
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

    private async Task SearchUserAsync(string username)
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

    private async Task ExpectUserExistsAsync(string username)
    {
        await Expect(TableBody).ToContainTextAsync(username);
        await Expect(Page.Locator("text=(1) Record Found")).ToBeVisibleAsync();
    }

    private async Task DeleteFirstSearchResultAsync()
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
    
    private async Task EditFirstSearchResultAsync(string changedUsername)
    {
        await Page.Locator(".oxd-table-cell-actions")
            .Locator("button")
            .Nth(1)
            .ClickAsync();

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Page.Locator(".oxd-checkbox-input").ClickAsync();
        await Page.Locator(".user-password-row").WaitForAsync();

        await PasswordInput.FillAsync(ChangePassword);
        await ConfirmPasswordInput.FillAsync(ChangePassword);
        await UsernameInput.FillAsync(changedUsername);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Page.GetByText("Successfully Updated", new() { Exact = true }).WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    private async Task ExpectUserDoesNotExistAsync(string username)
    {
        await Expect(NoRecordsFoundText).ToBeVisibleAsync();
        await Expect(TableBody).Not.ToContainTextAsync(username);
    }
}
