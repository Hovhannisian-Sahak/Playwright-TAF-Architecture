using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Models;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.Tests.TestData;

namespace PlaywrightTAF.Tests.ApiTests;

public class ArticleApiTests : BaseApiTest
{
    [Test]
    [Category("API")]
    public async Task CreateArticle_ShouldReturnCreatedArticle()
    {
        var article = ArticleTestDataBuilder
            .New()
            .WithTitlePrefix("Created API Article")
            .WithTags("taf", "API", "create")
            .Build();

        var createdArticle = await CreateArticleAsync(article);

        AssertArticleMatches(createdArticle, article);
    }

    [Test]
    [Category("API")]
    public async Task GetArticle_ShouldReturnCreatedArticle()
    {
        var createdArticle = await CreateTestArticle();

        var fetchedArticle = await ArticleService.GetArticle(createdArticle.slug);

        Assert.Multiple(() =>
        {
            Assert.That(fetchedArticle.slug, Is.EqualTo(createdArticle.slug));
            Assert.That(fetchedArticle.title, Is.EqualTo(createdArticle.title));
            Assert.That(fetchedArticle.description, Is.EqualTo(createdArticle.description));
            Assert.That(fetchedArticle.body, Is.EqualTo(createdArticle.body));
        });
    }

    [Test]
    [Category("API")]
    public async Task UpdateArticle_ShouldReturnUpdatedArticle()
    {
        var createdArticle = await CreateTestArticle();

        var updatedArticleData = ArticleTestDataBuilder
            .New()
            .WithTitle($"Updated {createdArticle.title}")
            .WithDescription("Article updated by API automation.")
            .WithBody("Updated article body.")
            .WithTags("taf", "API", "Updated")
            .Build();

        var updatedArticle = await ArticleService.UpdateArticle(
            createdArticle.slug,
            updatedArticleData.Title,
            updatedArticleData.Description,
            updatedArticleData.Body,
            updatedArticleData.Tags);
        UntrackArticleFromCleanup(createdArticle.slug);
        TrackArticleForCleanup(updatedArticle.slug);

        Assert.Multiple(() =>
        {
            Assert.That(updatedArticle.slug, Is.Not.Empty);
            Assert.That(updatedArticle.slug, Is.Not.EqualTo(createdArticle.slug));
            Assert.That(updatedArticle.title, Is.EqualTo(updatedArticleData.Title));
            Assert.That(updatedArticle.description, Is.EqualTo(updatedArticleData.Description));
            Assert.That(updatedArticle.body, Is.EqualTo(updatedArticleData.Body));
            Assert.That(updatedArticle.tagList, Is.EquivalentTo(updatedArticleData.Tags));
        });
    }

    [Test]
    [Category("API")]
    public async Task FavoriteArticle_ShouldMarkArticleAsFavorited()
    {
        var createdArticle = await CreateTestArticle();

        var favoritedArticle = await ArticleService.FavoriteArticle(createdArticle.slug);

        Assert.Multiple(() =>
        {
            Assert.That(favoritedArticle.slug, Is.EqualTo(createdArticle.slug));
            Assert.That(favoritedArticle.favorited, Is.True);
            Assert.That(favoritedArticle.favoritesCount, Is.EqualTo(createdArticle.favoritesCount + 1));
        });
    }

    [Test]
    [Category("API")]
    public async Task UnfavoriteArticle_ShouldMarkArticleAsNotFavorited()
    {
        var createdArticle = await CreateTestArticle();

        var favoritedArticle = await ArticleService.FavoriteArticle(createdArticle.slug);
        var unfavoritedArticle = await ArticleService.UnfavoriteArticle(createdArticle.slug);

        Assert.Multiple(() =>
        {
            Assert.That(favoritedArticle.favorited, Is.True);
            Assert.That(unfavoritedArticle.slug, Is.EqualTo(createdArticle.slug));
            Assert.That(unfavoritedArticle.favorited, Is.False);
            Assert.That(unfavoritedArticle.favoritesCount, Is.EqualTo(favoritedArticle.favoritesCount - 1));
        });
    }

    [Test]
    [Category("API")]
    public async Task DeleteArticle_ShouldDeleteCreatedArticle()
    {
        var createdArticle = await CreateTestArticle();

        Assert.DoesNotThrowAsync(async () => await DeleteTrackedArticleAsync(createdArticle.slug));
    }

    private Task<ArticleData> CreateTestArticle()
    {
        return CreateArticleAsync(ArticleTestDataBuilder.New().Build());
    }

    private async Task<ArticleData> CreateArticleAsync(ArticleTestData article)
    {
        var createdArticle = await ArticleService.CreateArticle(
            article.Title,
            article.Description,
            article.Body,
            article.Tags);

        TrackArticleForCleanup(createdArticle.slug);

        return createdArticle;
    }

    private static void AssertArticleMatches(ArticleData actual, ArticleTestData expected)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.slug, Is.Not.Empty);
            Assert.That(actual.title, Is.EqualTo(expected.Title));
            Assert.That(actual.description, Is.EqualTo(expected.Description));
            Assert.That(actual.body, Is.EqualTo(expected.Body));
            Assert.That(actual.tagList, Is.EquivalentTo(expected.Tags));
        });
    }
}
