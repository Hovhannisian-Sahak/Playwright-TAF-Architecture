using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Tests.DependencyInjection;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.UserManagementPages;

namespace PlaywrightTAF.Tests.Authentication;

public static class AuthSetup
{
    public static async Task EnsureUserExistsAsync(Credentials userCredentials, string employeeName)
    {
        await RunWithAuthenticatedPageAsync(async (configuration, services, context) =>
        {
            var loginPage = services.GetRequiredService<LoginPage>();
            await loginPage.LoginAsync(configuration.Admin.Username, configuration.Admin.Password);

            var deleteUserPage = services.GetRequiredService<DeleteUserPage>();
            await deleteUserPage.OpenUserManagementAsync();
            await deleteUserPage.SearchUserAsync(userCredentials.Username);

            if (await deleteUserPage.IsUserListedAsync(userCredentials.Username))
            {
                var editUserPage = services.GetRequiredService<EditUserPage>();
                await editUserPage.EditFirstSearchResultAsync(userCredentials.Username, userCredentials.Password, "ESS");
            }
            else
            {
                var addUserPage = services.GetRequiredService<AddUserPage>();
                await addUserPage.OpenAddUserFormAsync();
                await addUserPage.CreateUserAsync("ESS", userCredentials.Username, employeeName, userCredentials.Password);
            }
        });
    }

    public static async Task CreateAuthStateAsync(Credentials credentials, string storageStatePath)
    {
        AuthStatePaths.EnsureDirectoryExists();

        await RunWithAuthenticatedPageAsync(async (configuration, services, context) =>
        {
            var loginPage = services.GetRequiredService<LoginPage>();

            await loginPage.LoginAsync(credentials.Username, credentials.Password);

            await context.StorageStateAsync(new() { Path = storageStatePath });
        });
    }

    private static async Task RunWithAuthenticatedPageAsync(
        Func<AppConfiguration, IServiceProvider, IBrowserContext, Task> action)
    {
        using var playwright = await Playwright.CreateAsync();

        var configuration = ConfigurationReader.Current;

        IBrowserType browserType = configuration.Browser.ToLowerInvariant() switch
        {
            "firefox" => playwright.Firefox,
            "webkit" => playwright.Webkit,
            _ => playwright.Chromium
        };

        var browser = await browserType.LaunchAsync(
            new()
            {
                Headless = true
            });

        var context = await browser.NewContextAsync(
            new()
            {
                BaseURL = configuration.BaseUrl
            });

        try
        {
            var page = await context.NewPageAsync();
            page.SetDefaultTimeout(configuration.DefaultTimeoutMilliseconds);

            await page.GotoAsync(
                configuration.BaseUrl,
                new()
                {
                    WaitUntil = WaitUntilState.Commit,
                    Timeout = configuration.DefaultTimeoutMilliseconds * 2
                });

            using var services = new ServiceCollection()
                .AddSingleton(page)
                .AddUiPageObjects()
                .BuildServiceProvider();

            await action(configuration, services, context);
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }
}
