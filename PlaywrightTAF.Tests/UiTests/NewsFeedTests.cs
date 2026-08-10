using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages.NewsFeedPages;

namespace PlaywrightTAF.Tests.UiTests;

public class NewsFeedTests : AdminTest
{
    private readonly MostLikedPostsPage mostLikedPostsPage;

    public NewsFeedTests()
    {
        mostLikedPostsPage = PageObject<MostLikedPostsPage>();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanViewMostLikedPosts()
    {
        await mostLikedPostsPage.OpenAsync();
        var isCorrectlySorted = await mostLikedPostsPage.GetMostLikedPostsAsync();
        Assert.That(isCorrectlySorted, Is.True, "The posts are not sorted by most liked.");
    }
}
