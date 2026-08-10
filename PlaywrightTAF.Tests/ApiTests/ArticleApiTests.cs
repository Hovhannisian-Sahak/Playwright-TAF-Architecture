using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Models;
using PlaywrightTAF.Tests.Assertions;
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

        ArticleAssertions.ShouldMatch(createdArticle, article);
    }

    [Test]
    [Category("API")]
    public async Task GetArticle_ShouldReturnCreatedArticle()
    {
        var createdArticle = await CreateTestArticle();

        var fetchedArticle = await ArticleService.GetArticle(createdArticle.slug);

        ArticleAssertions.ShouldMatchCreatedArticle(fetchedArticle, createdArticle);
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

        ArticleAssertions.ShouldBeUpdatedFrom(updatedArticle, createdArticle, updatedArticleData);
    }

    [Test]
    [Category("API")]
    public async Task FavoriteArticle_ShouldMarkArticleAsFavorited()
    {
        var createdArticle = await CreateTestArticle();

        var favoritedArticle = await ArticleService.FavoriteArticle(createdArticle.slug);

        ArticleAssertions.ShouldBeFavoritedFrom(favoritedArticle, createdArticle);
    }

    [Test]
    [Category("API")]
    public async Task UnfavoriteArticle_ShouldMarkArticleAsNotFavorited()
    {
        var createdArticle = await CreateTestArticle();

        var favoritedArticle = await ArticleService.FavoriteArticle(createdArticle.slug);
        var unfavoritedArticle = await ArticleService.UnfavoriteArticle(createdArticle.slug);

        Assert.That(favoritedArticle.favorited, Is.True);
        ArticleAssertions.ShouldBeUnfavoritedFrom(unfavoritedArticle, favoritedArticle);
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

}
