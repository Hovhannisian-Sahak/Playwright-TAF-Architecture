using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages.NewsFeedPages;

namespace PlaywrightTAF.Tests.UiTests;

public class NewsFeedTests : AdminTest
{
    private MostLikedPostsPage MostLikedPostsPage => PageObject<MostLikedPostsPage>();

    [Test]
    [Category("UI")]
    public async Task AdminCanViewMostLikedPosts()
    {
        await MostLikedPostsPage.OpenAsync();
        var isCorrectlySorted = await MostLikedPostsPage.GetMostLikedPostsAsync();
        Assert.That(isCorrectlySorted, Is.True, "The posts are not sorted by most liked.");
    }
}
