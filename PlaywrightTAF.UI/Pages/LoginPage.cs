using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Core.Logging;
using Serilog;

namespace PlaywrightTAF.UI.Pages;

public class LoginPage : BasePage
{
    private static readonly ILogger Logger = LogProvider.ForContext<LoginPage>();

    public LoginPage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => ConfigurationReader.Current.BaseUrl;

    private ILocator UsernameInput => Page.GetByPlaceholder("username");
    private ILocator PasswordInput => Page.GetByPlaceholder("password");
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator UserDropdownName => Page.Locator(".oxd-userdropdown-name");

    public Task OpenLoginPageAsync()
    {
        return OpenAsync();
    }

    public override async Task<bool> IsLoadedAsync()
    {
        return await UsernameInput.IsVisibleAsync()
               && await PasswordInput.IsVisibleAsync()
               && await LoginButton.IsVisibleAsync();
    }

    public async Task LoginAsync(string username, string password)
    {
        Logger.Information("Logging in as {Username}", username);
        await FillAndExpectValueAsync(UsernameInput, username);
        await FillAndExpectValueAsync(PasswordInput, password);
        await LoginButton.ClickAsync();
        await WaitUntilVisibleAsync(UserDropdownName);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Logger.Information("Login submitted for {Username}; current URL is {CurrentUrl}", username, CurrentUrl);
    }
}
