using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Pages.NewsFeedPages.Base;

public class NewsFeedBasePage : BasePage
{
    public NewsFeedBasePage(IPage page) : base(page)
    {
    }

    private ILocator NewsFeedMostLikedPosts => Page.Locator(".orangehrm-buzz-newsfeed-posts");
    private ILocator MostRecentPostsButton => Page.GetByRole(AriaRole.Button, new() { Name = "Most Recent Posts" });
    private ILocator MostCommentedPostsButton => Page.GetByRole(AriaRole.Button, new() { Name = "Most Commented Posts" });
    private ILocator MostLikedPostsButton => Page.GetByRole(AriaRole.Button, new() { Name = "Most Liked Posts" });

    protected override string PageUrl => BuildUrl(ConfigurationReader.Current.BaseUrl, "/web/index.php/buzz/viewBuzz");
    
    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(CurrentUrl.Contains("viewBuzz", StringComparison.OrdinalIgnoreCase));
    }
    
    public async Task ClickMostRecentPostsButtonAsync()
    {
        await MostRecentPostsButton.ClickAsync();
    }
    
    public async Task ClickMostCommentedPostsButtonAsync()
    {
        await MostCommentedPostsButton.ClickAsync();
    }
    
    public async Task ClickMostLikedPostsButtonAsync()
    {
        await MostLikedPostsButton.ClickAsync();
        await WaitUntilVisibleAsync(NewsFeedMostLikedPosts);
    }
}