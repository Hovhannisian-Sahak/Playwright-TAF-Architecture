﻿using System.Threading.Tasks;
using Microsoft.Playwright;
using System;
using System.IO;
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

    [Test]
    [Category("UI")]
    public async Task AdminCanEditInfo()
    {
        var lastName = $"Admin{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await OpenMyInfoAsync();
        await FillLastNameAsync(lastName);
        await SelectNationalityAsync("Armenian");
        await SetBirthDateAsync();
        await SavePersonalDetailsAsync();
        await ClickToUploadFile();
        await UploadFileAndMakeCommentAsync();
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

    private async Task OpenMyInfoAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Info" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Personal Details" })).ToBeVisibleAsync();
    }

    private async Task FillLastNameAsync(string lastName)
    {
        var lastNameInput = Page.Locator("input[name='lastName']");

        await lastNameInput.ClearAsync();
        await lastNameInput.FillAsync(lastName);
        await Expect(lastNameInput).ToHaveValueAsync(lastName);
    }

    private async Task SelectNationalityAsync(string nationality)
    {
        await SelectDropdownOptionAsync(0, nationality);
        await Expect(Page.Locator(".oxd-select-wrapper").Nth(0)).ToContainTextAsync(nationality);
    }

    private async Task SavePersonalDetailsAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).First.ClickAsync();
        await Page.GetByText("Successfully Updated", new() { Exact = true }).WaitForAsync();
    }

    private async Task ClickToUploadFile()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add" }).ClickAsync();
        await Page.Locator(".orangehrm-card-container").Nth(2).WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });
    }
    private async Task UploadFileAndMakeCommentAsync()
    {
        var fileChooserTask = Page.WaitForFileChooserAsync();
        await Page.Locator(".oxd-file-button").ClickAsync();
        var chooser = await fileChooserTask;
        string filePath = Path.Combine(
            AppContext.BaseDirectory,
            "test.png");

        Assert.That(File.Exists(filePath), Is.True, $"Upload test file was not found: {filePath}");

        await chooser.SetFilesAsync(filePath);

        await Expect(Page.Locator(".oxd-file-input-div")).ToContainTextAsync("test.png");

        await Page.GetByPlaceholder("Type comment here").FillAsync("Test");
        await Expect(Page.GetByPlaceholder("Type comment here")).ToHaveValueAsync("Test");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).Nth(2).ClickAsync();
        await Page.GetByText("Successfully Saved", new() { Exact = true }).WaitForAsync();
    }
    
    private async Task SetBirthDateAsync()
    {
        const string expectedBirthDate = "2025-19-11";
        var birthDateInput = Page.Locator(".oxd-date-input input").Nth(1);

        // click to open calendar
        await Page.Locator(".oxd-date-input i").Nth(1).ClickAsync();
        // set year
        await Page.Locator(".oxd-calendar-selector-year-selected > .oxd-icon").ClickAsync();
        await Page.GetByRole(AriaRole.Menu).GetByText("2025", new() { Exact = true }).ClickAsync();
        //set month
        await Page.Locator(".oxd-calendar-selector-month-selected > .oxd-icon").ClickAsync();
        await Page.GetByRole(AriaRole.Menu).GetByText("November", new() { Exact = true }).ClickAsync();
        // set day
        await Page.Locator(".oxd-calendar-date").GetByText("19", new() { Exact = true }).ClickAsync();
        // assertion
        await Expect(birthDateInput).ToHaveValueAsync(expectedBirthDate);
    }
}
