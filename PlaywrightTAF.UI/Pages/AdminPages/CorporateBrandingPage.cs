using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Pages.AdminPages.Base;

namespace PlaywrightTAF.UI.Pages.AdminPages;

public class AdminCorporateBrandingPage : BasePageAdmin
{
    private const string CorporateBrandingPath = "/web/index.php/admin/addTheme";

    ILocator CorporateBrandingHeader => Page.Locator("text=Corporate Branding");

    private ILocator ColorPickerButton => Page.Locator(".orangehrm-color-input-wrapper")
        .Filter(new() { HasText = "Secondary Font Color" })
        .Locator(".oxd-color-input-preview");
    private ILocator ColorPicker => Page.Locator(".oxd-color-picker");
    private ILocator ColorPickerInput => Page.Locator(".oxd-color-picker")
        .Locator(".oxd-input");

    private ILocator PublishButton => Page.GetByRole(AriaRole.Button, new() { Name = "Publish" });
    private ILocator SuccessfullyUpdatedText => Page.GetByText("Successfully Updated", new() { Exact = true });

    protected override string PageUrl => new Uri(new Uri(ConfigurationReader.Current.BaseUrl), CorporateBrandingPath).ToString();
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
        await ColorPicker.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });

        var currentColor = await ColorPickerInput.InputValueAsync();
        var nextColor = currentColor.Equals("#ff0000", StringComparison.OrdinalIgnoreCase)
            ? "#00ff00"
            : "#ff0000";

        await ColorPickerInput.FillAsync(nextColor);
    }

    public async Task ClickPublishAsync()
    {
        await PublishButton.ClickAsync();
        await SuccessfullyUpdatedText.WaitForAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
