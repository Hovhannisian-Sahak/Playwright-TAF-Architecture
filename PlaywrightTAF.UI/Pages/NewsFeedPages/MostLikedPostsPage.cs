using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.NewsFeedPages.Base;

namespace PlaywrightTAF.UI.Pages.NewsFeedPages;

public class MostLikedPostsPage : NewsFeedBasePage
{
    private readonly NewsFeedBasePage newsFeedBasePage;
    private ILocator LikeCounts => Page.GetByText("Likes");
    public MostLikedPostsPage(IPage page, NewsFeedBasePage newsFeedBasePage) : base(page)
    {
        this.newsFeedBasePage = newsFeedBasePage;
    }

    public async Task<bool> GetMostLikedPostsAsync()
    {
        await newsFeedBasePage.ClickMostLikedPostsButtonAsync();

        var likeTexts = await LikeCounts.AllTextContentsAsync();

        var actualLikes = likeTexts
            .TakeLast(10)
            .Select(text =>
                int.Parse(Regex.Match(text, @"\d+").Value))
            .ToList();

        var expectedLikes = actualLikes
            .OrderByDescending(count => count)
            .ToList();

        return expectedLikes.SequenceEqual(actualLikes);
    }
}
