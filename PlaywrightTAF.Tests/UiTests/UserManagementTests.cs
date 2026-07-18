﻿using System.Threading.Tasks;
using Microsoft.Playwright;
using System;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using static Microsoft.Playwright.Assertions;
namespace PlaywrightTAF.Tests.UiTests;

public class UserManagementTests : UiBaseTest
{
    [Test]
    [Category("UI")]
    public async Task AdminCanAddUser()
    {
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        // click Admin button
        await Page.Locator(".oxd-main-menu li").First.ClickAsync();
        // click Add button
        await Page.GetByRole(AriaRole.Button, new() { Name = " Add " }).ClickAsync();
        // select user role
        await Page.Locator(".oxd-select-wrapper")
            .Nth(0)
            .ClickAsync();

        await Page.GetByRole(AriaRole.Listbox)
            .GetByText("Admin", new()
            {
                Exact = true
            })
            .ClickAsync();
       // type for hints
       await Page
           .Locator("input[placeholder='Type for hints...']")
           .FillAsync("Ranga  Akunuri");
       
       await Page.Locator(".oxd-autocomplete-option")
           .Filter(new() { HasText = "Ranga  Akunuri" })
           .First
           .ClickAsync();
        // select status
        await Page.Locator(".oxd-select-wrapper")
            .Nth(1)
            .ClickAsync();

        await Page.GetByRole(AriaRole.Listbox)
            .GetByText("Enabled", new()
            {
                Exact = true
            })
            .ClickAsync();
        // fill username
        var username = Page
            .Locator(".oxd-input-group")
            .Filter(new() { HasText = "Username" })
            .Locator("input");

        await username.FillAsync(newUsername);
        // fill password
        var password = Page.Locator("input[type='password']").Nth(0);
        await password.FillAsync("TestUser123!@#Aa");
        // confirm password
        var confirmPassword = Page.Locator("input[type='password']").Nth(1);
        await confirmPassword.FillAsync("TestUser123!@#Aa");
        // click Save button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        // wait for success message
        await Page.GetByText("Success", new() { Exact = true }).WaitForAsync();
        // wait for page load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        // fill search username
        var searchUsername = Page
            .Locator(".oxd-table-filter .oxd-input-group")
            .Filter(new() { HasText = "Username" })
            .Locator("input");

        await searchUsername.FillAsync(newUsername);
        await Expect(searchUsername).ToHaveValueAsync(newUsername);
        // submit search
        await Page
            .Locator(".oxd-table-filter")
            .GetByRole(AriaRole.Button, new() { Name = "Search" })
            .ClickAsync();
        // assert search result exists
        await Expect(
            Page.Locator(".oxd-table-body")
        ).ToContainTextAsync(newUsername);
        
        // assert search result count is 1
        await Expect(
            Page.Locator("text=(1) Record Found")
        ).ToBeVisibleAsync();
        
        // delete created user
        await Page
            .Locator(".oxd-table-cell-actions").Locator("button").Nth(0)
            .ClickAsync();
        
        // confirm deletion
        await Page
            .Locator(".orangehrm-modal-footer").Locator("button").Nth(1)
            .ClickAsync();
        
        // wait for success message
        await Page.GetByText("Successfully Deleted", new() { Exact = true }).WaitForAsync();
        // wait for page load
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        // search for DELETED user

        await searchUsername.FillAsync(newUsername);
        await Expect(searchUsername).ToHaveValueAsync(newUsername);
        // submit search
        await Page
            .Locator(".oxd-table-filter")
            .GetByRole(AriaRole.Button, new() { Name = "Search" })
            .ClickAsync();
        // assert no record result
        await Expect(Page.Locator(".orangehrm-horizontal-padding"))
            .ToContainTextAsync("No Records Found");

        // assert search result does not exist
        await Expect(
            Page.Locator(".oxd-table-body")
        ).Not.ToContainTextAsync(newUsername);

    }
}
