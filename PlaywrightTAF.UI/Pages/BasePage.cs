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

    protected async Task UploadFileAsync(ILocator fileButton, ILocator fileInput, string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Upload test file was not found: {filePath}", filePath);
        }

        var fileChooserTask = Page.WaitForFileChooserAsync();

        await fileButton.WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });
        await fileButton.ClickAsync();

        var chooser = await fileChooserTask;
        await chooser.SetFilesAsync(filePath);

        await Expect(fileInput).ToContainTextAsync(Path.GetFileName(filePath));
    }

    public string CurrentUrl => Page.Url;
}
