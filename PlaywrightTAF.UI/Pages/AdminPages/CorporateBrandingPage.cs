using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.AdminPages.Base;

namespace PlaywrightTAF.UI.Pages.AdminPages;

public class AdminCorporateBrandingPage : BasePageAdmin
{
    private const string CorporateBrandingPath = "/web/index.php/admin/addTheme";

    private ILocator CorporateBrandingHeader => Page.GetByRole(AriaRole.Heading, new() { Name = "Corporate Branding" });

    private ILocator ColorPickerButton => Page.Locator(".orangehrm-color-input-wrapper")
        .Filter(new() { HasText = "Secondary Font Color" })
        .Locator(".oxd-color-input-preview");
    private ILocator ColorPicker => Page.Locator(".oxd-color-picker");
    private ILocator ColorPickerInput => Page.Locator(".oxd-color-picker")
        .Locator(".oxd-input");

    private ILocator PublishButton => Page.GetByRole(AriaRole.Button, new() { Name = "Publish" });
    private ILocator ResetToDefaultButton => Page.GetByRole(AriaRole.Button, new() { Name = "Reset to Default" });
    private ILocator SuccessfullySavedText => Page.GetByText("Successfully Saved", new() { Exact = true });
    private ILocator FileButton => Page.Locator(".oxd-file-button").Nth(0);
    private ILocator FileInput => Page.Locator(".oxd-file-input-div").Nth(0);

    protected override string PageUrl => BuildUrl(ConfigurationReader.Current.BaseUrl, CorporateBrandingPath);

    public override async Task<bool> IsLoadedAsync()
    {
        return await CorporateBrandingHeader.IsVisibleAsync();
    }

    public AdminCorporateBrandingPage(IPage page) : base(page)
    {
    }

    public async Task ChooseColorAsync()
    {
        await ColorPickerButton.ClickAsync();
        await WaitUntilVisibleAsync(ColorPicker);

        var currentColor = await ColorPickerInput.InputValueAsync();
        var nextColor = currentColor.Equals("#ff0000", StringComparison.OrdinalIgnoreCase)
            ? "#00ff00"
            : "#ff0000";

        await ColorPickerInput.FillAsync(nextColor);
        await Page.Keyboard.PressAsync("Escape");
        await ColorPicker.WaitForAsync(new()
        {
            State = WaitForSelectorState.Hidden
        });
    }

    public async Task ResetToDefaultAsync()
    {
        await ResetToDefaultButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ChooseClientLogoAsync(string filePath)
    {
        await UploadFileAsync(FileButton, FileInput, filePath);
    }

    public async Task ClickPublishAsync()
    {
        await PublishButton.ClickAsync();
    }

    public async Task ExpectSuccessfullySavedAsync()
    {
        await Expect(SuccessfullySavedText).ToBeVisibleAsync(new()
        {
            Timeout = ConfigurationReader.Current.DefaultTimeoutMilliseconds
        });

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
