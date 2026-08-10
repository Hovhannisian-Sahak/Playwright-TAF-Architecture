using NUnit.Framework;
using PlaywrightTAF.Core.Models;
using PlaywrightTAF.Tests.TestData;

namespace PlaywrightTAF.Tests.Assertions;

public static class ArticleAssertions
{
    public static void ShouldMatch(ArticleData actual, ArticleTestData expected)
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

    public static void ShouldMatchCreatedArticle(ArticleData actual, ArticleData expected)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.slug, Is.EqualTo(expected.slug));
            Assert.That(actual.title, Is.EqualTo(expected.title));
            Assert.That(actual.description, Is.EqualTo(expected.description));
            Assert.That(actual.body, Is.EqualTo(expected.body));
        });
    }

    public static void ShouldBeUpdatedFrom(ArticleData actual, ArticleData original, ArticleTestData expected)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.slug, Is.Not.Empty);
            Assert.That(actual.slug, Is.Not.EqualTo(original.slug));
            Assert.That(actual.title, Is.EqualTo(expected.Title));
            Assert.That(actual.description, Is.EqualTo(expected.Description));
            Assert.That(actual.body, Is.EqualTo(expected.Body));
            Assert.That(actual.tagList, Is.EquivalentTo(expected.Tags));
        });
    }

    public static void ShouldBeFavoritedFrom(ArticleData actual, ArticleData original)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.slug, Is.EqualTo(original.slug));
            Assert.That(actual.favorited, Is.True);
            Assert.That(actual.favoritesCount, Is.EqualTo(original.favoritesCount + 1));
        });
    }

    public static void ShouldBeUnfavoritedFrom(ArticleData actual, ArticleData favoritedArticle)
    {
        Assert.Multiple(() =>
        {
            Assert.That(actual.slug, Is.EqualTo(favoritedArticle.slug));
            Assert.That(actual.favorited, Is.False);
            Assert.That(actual.favoritesCount, Is.EqualTo(favoritedArticle.favoritesCount - 1));
        });
    }
}
