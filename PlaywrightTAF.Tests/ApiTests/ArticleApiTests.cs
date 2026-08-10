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
        string? createdSlug = null;

        try
        {
            var createdArticle = await CreateArticleAsync(article);
            createdSlug = createdArticle.slug;

            AssertArticleMatches(createdArticle, article);
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(createdSlug))
            {
                await ArticleService.DeleteArticle(createdSlug);
            }
        }
    }

    [Test]
    [Category("API")]
    public async Task GetArticle_ShouldReturnCreatedArticle()
    {
        var createdArticle = await CreateTestArticle();
        string createdSlug = createdArticle.slug;

        try
        {
            var fetchedArticle = await ArticleService.GetArticle(createdSlug);

            Assert.Multiple(() =>
            {
                Assert.That(fetchedArticle.slug, Is.EqualTo(createdSlug));
                Assert.That(fetchedArticle.title, Is.EqualTo(createdArticle.title));
                Assert.That(fetchedArticle.description, Is.EqualTo(createdArticle.description));
                Assert.That(fetchedArticle.body, Is.EqualTo(createdArticle.body));
            });
        }
        finally
        {
            await ArticleService.DeleteArticle(createdSlug);
        }
    }

    [Test]
    [Category("API")]
    public async Task UpdateArticle_ShouldReturnUpdatedArticle()
    {
        var createdArticle = await CreateTestArticle();
        string currentSlug = createdArticle.slug;

        try
        {
            var updatedArticleData = ArticleTestDataBuilder
                .New()
                .WithTitle($"Updated {createdArticle.title}")
                .WithDescription("Article updated by API automation.")
                .WithBody("Updated article body.")
                .WithTags("taf", "API", "Updated")
                .Build();

            var updatedArticle = await ArticleService.UpdateArticle(
                currentSlug,
                updatedArticleData.Title,
                updatedArticleData.Description,
                updatedArticleData.Body,
                updatedArticleData.Tags);
            currentSlug = updatedArticle.slug;

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
        finally
        {
            await ArticleService.DeleteArticle(currentSlug);
        }
    }

    [Test]
    [Category("API")]
    public async Task FavoriteArticle_ShouldMarkArticleAsFavorited()
    {
        var createdArticle = await CreateTestArticle();
        string createdSlug = createdArticle.slug;

        try
        {
            var favoritedArticle = await ArticleService.FavoriteArticle(createdSlug);

            Assert.Multiple(() =>
            {
                Assert.That(favoritedArticle.slug, Is.EqualTo(createdSlug));
                Assert.That(favoritedArticle.favorited, Is.True);
                Assert.That(favoritedArticle.favoritesCount, Is.EqualTo(createdArticle.favoritesCount + 1));
            });
        }
        finally
        {
            await ArticleService.DeleteArticle(createdSlug);
        }
    }

    [Test]
    [Category("API")]
    public async Task UnfavoriteArticle_ShouldMarkArticleAsNotFavorited()
    {
        var createdArticle = await CreateTestArticle();
        string createdSlug = createdArticle.slug;

        try
        {
            var favoritedArticle = await ArticleService.FavoriteArticle(createdSlug);
            var unfavoritedArticle = await ArticleService.UnfavoriteArticle(createdSlug);

            Assert.Multiple(() =>
            {
                Assert.That(favoritedArticle.favorited, Is.True);
                Assert.That(unfavoritedArticle.slug, Is.EqualTo(createdSlug));
                Assert.That(unfavoritedArticle.favorited, Is.False);
                Assert.That(unfavoritedArticle.favoritesCount, Is.EqualTo(favoritedArticle.favoritesCount - 1));
            });
        }
        finally
        {
            await ArticleService.DeleteArticle(createdSlug);
        }
    }

    [Test]
    [Category("API")]
    public async Task DeleteArticle_ShouldDeleteCreatedArticle()
    {
        var createdArticle = await CreateTestArticle();

        Assert.DoesNotThrowAsync(async () => await ArticleService.DeleteArticle(createdArticle.slug));
    }

    private Task<ArticleData> CreateTestArticle()
    {
        return CreateArticleAsync(ArticleTestDataBuilder.New().Build());
    }

    private Task<ArticleData> CreateArticleAsync(ArticleTestData article)
    {
        return ArticleService.CreateArticle(
            article.Title,
            article.Description,
            article.Body,
            article.Tags);
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
