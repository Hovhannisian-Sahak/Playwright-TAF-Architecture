using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages.NewsFeedPages;

namespace PlaywrightTAF.Tests.UiTests;

public class NewsFeedTests : AdminTest
{
    [Test]
    [Category("UI")]
    public async Task AdminCanViewMostLikedPosts()
    {
        var mostLikedPostsPage = new MostLikedPostsPage(Page);
        await mostLikedPostsPage.OpenAsync();
        var isCorrectlySorted = await mostLikedPostsPage.GetMostLikedPostsAsync();
        Assert.That(isCorrectlySorted, Is.True, "The posts are not sorted by most liked.");
    }
}