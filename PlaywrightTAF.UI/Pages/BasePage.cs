using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Pages;

public abstract class BasePage
{
    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected IPage Page { get; }

    protected abstract string PageUrl { get; }

    public virtual async Task OpenAsync()
    {
        await Page.GotoAsync(PageUrl);
        await WaitForPageLoadAsync();
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
