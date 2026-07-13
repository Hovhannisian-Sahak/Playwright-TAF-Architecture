using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages;

public class LoginPage : BasePage
{
    public LoginPage(IPage page) : base(page)
    {
    }
    protected override string PageUrl => ConfigurationReader.Current.BaseUrl;

    private ILocator UsernameInput => Page.GetByPlaceholder("username");
    private ILocator PasswordInput => Page.GetByPlaceholder("password");
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Login" });

    public Task OpenLoginPageAsync()
    {
        return OpenAsync();
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
        await Page.WaitForLoadStateAsync(
            LoadState.NetworkIdle);
    }
}