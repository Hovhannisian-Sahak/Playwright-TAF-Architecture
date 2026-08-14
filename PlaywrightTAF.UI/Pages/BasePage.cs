using Microsoft.Playwright;
using PlaywrightTAF.Core.Logging;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages;

public abstract class BasePage
{
    private static readonly ILogger Logger = LogProvider.ForContext<BasePage>();

    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected IPage Page { get; }

    protected abstract string PageUrl { get; }

    public virtual async Task OpenAsync()
    {
        Logger.Information("Opening page {PageUrl}", PageUrl);
        await Page.GotoAsync(PageUrl);
        await WaitForPageLoadAsync();
        Logger.Information("Opened page {CurrentUrl}", CurrentUrl);
    }

    public virtual Task WaitForPageLoadAsync()
    {
        return Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public abstract Task<bool> IsLoadedAsync();

    public Task<string> GetTitleAsync()
    {
        return Page.TitleAsync();
    }

    protected static string BuildUrl(string baseUrl, string path)
    {
        return new Uri(new Uri(baseUrl), path).ToString();
    }

    protected async Task FillAndExpectValueAsync(ILocator input, string value)
    {
        await input.FillAsync(value);
        await Expect(input).ToHaveValueAsync(value);
    }

    protected async Task ClearFillAndExpectValueAsync(ILocator input, string value)
    {
        await input.ClearAsync();
        await FillAndExpectValueAsync(input, value);
    }

    protected Task WaitUntilVisibleAsync(ILocator locator)
    {
        return locator.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });
    }

    protected async Task ClickWhenVisibleAsync(ILocator locator)
    {
        await WaitUntilVisibleAsync(locator);
        await locator.ClickAsync();
    }

    protected async Task ClickWhenVisibleAndWaitForPageLoadAsync(ILocator locator)
    {
        await ClickWhenVisibleAsync(locator);
        await WaitForPageLoadAsync();
    }

    protected async Task SelectDropdownOptionAsync(ILocator dropdowns, int dropdownIndex, string option)
    {
        await dropdowns.Nth(dropdownIndex).ClickAsync();

        await Page.GetByRole(AriaRole.Listbox)
            .GetByText(option, new() { Exact = true })
            .ClickAsync();
    }

    protected async Task UploadFileAsync(ILocator fileButton, ILocator fileInput, string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Upload test file was not found: {filePath}", filePath);
        }

        var fileChooserTask = Page.WaitForFileChooserAsync();

        await WaitUntilVisibleAsync(fileButton);
        await fileButton.ClickAsync();

        var chooser = await fileChooserTask;
        await chooser.SetFilesAsync(filePath);

        await Expect(fileInput).ToContainTextAsync(Path.GetFileName(filePath));
    }

    public string CurrentUrl => Page.Url;

    public Task<T> EvaluateAsync<T>(string expression)
    {
        return Page.EvaluateAsync<T>(expression);
    }

    public Task EvaluateAsync(string expression)
    {
        return Page.EvaluateAsync(expression);
    }
}
