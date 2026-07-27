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
        // click button to choose color
        await ColorPickerButton.ClickAsync();
        // waiting color picker to be visible
        await ColorPicker.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });
        // fill color picker
        await ColorPickerInput.FillAsync("#ff0000");
    }

    public async Task ClickPublishAsync()
    {
        await PublishButton.ClickAsync();
    }
}
