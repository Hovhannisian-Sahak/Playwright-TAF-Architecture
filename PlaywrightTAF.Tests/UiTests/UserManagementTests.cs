﻿using System.Threading.Tasks;
using Microsoft.Playwright;
using System;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

namespace PlaywrightTAF.Tests.UiTests;

public class UserManagementTests : UiBaseTest
{
    [Test]
    [Category("UI")]
    public async Task UserCanOpenUserManagementPage()
    {
        var employeeName = await Page.Locator(".oxd-userdropdown-name").InnerTextAsync();

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
           .FillAsync(employeeName);
       
       await Page.Locator(".oxd-autocomplete-option")
           .Filter(new() { HasText = employeeName })
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

        await username.FillAsync($"taf{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}");
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
        // Assert.That(Page.Url,
        //     Does.Contain("admin/user-management"));
    }
}
