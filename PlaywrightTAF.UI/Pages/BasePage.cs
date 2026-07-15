using Microsoft.Playwright;
using PlaywrightTAF.Core.Logging;
using Serilog;

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

    public Task<string> GetTitleAsync()
    {
        return Page.TitleAsync();
    }

    public string CurrentUrl => Page.Url;
}
