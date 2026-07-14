using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

namespace PlaywrightTAF.Tests.ApiTests;

public class ArticleApiTests : BaseApiTest
{
    [Test]
    public async Task CreateArticle_ShouldReturnCreatedArticle()
    {
        string articleSuffix = Guid.NewGuid().ToString("N");
        string title = $"TAF Article {articleSuffix}";
        string description = "Article created by API automation.";
        string body = "Initial article body.";
        var tags = new List<string> { "taf", "API" };
        string? createdSlug = null;

        try
        {
            var createdArticle = await ArticleService.CreateArticle(title, description, body, tags);
            createdSlug = createdArticle.slug;

            Assert.Multiple(() =>
            {
                Assert.That(createdArticle.slug, Is.Not.Empty);
                Assert.That(createdArticle.title, Is.EqualTo(title));
                Assert.That(createdArticle.description, Is.EqualTo(description));
                Assert.That(createdArticle.body, Is.EqualTo(body));
                Assert.That(createdArticle.tagList, Is.EquivalentTo(tags));
            });
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
    public async Task UpdateArticle_ShouldReturnUpdatedArticle()
    {
        var createdArticle = await CreateTestArticle();
        string currentSlug = createdArticle.slug;

        try
        {
            string updatedTitle = $"Updated {createdArticle.title}";
            string updatedDescription = "Article updated by API automation.";
            string updatedBody = "Updated article body.";
            var updatedTags = new List<string> { "taf", "API", "Updated" };

            var updatedArticle = await ArticleService.UpdateArticle(
                currentSlug,
                updatedTitle,
                updatedDescription,
                updatedBody,
                updatedTags);
            currentSlug = updatedArticle.slug;

            Assert.Multiple(() =>
            {
                Assert.That(updatedArticle.slug, Is.Not.Empty);
                Assert.That(updatedArticle.slug, Is.Not.EqualTo(createdArticle.slug));
                Assert.That(updatedArticle.title, Is.EqualTo(updatedTitle));
                Assert.That(updatedArticle.description, Is.EqualTo(updatedDescription));
                Assert.That(updatedArticle.body, Is.EqualTo(updatedBody));
                Assert.That(updatedArticle.tagList, Is.EquivalentTo(updatedTags));
            });
        }
        finally
        {
            await ArticleService.DeleteArticle(currentSlug);
        }
    }

    [Test]
    public async Task DeleteArticle_ShouldDeleteCreatedArticle()
    {
        var createdArticle = await CreateTestArticle();

        Assert.DoesNotThrowAsync(async () => await ArticleService.DeleteArticle(createdArticle.slug));
    }

    private Task<PlaywrightTAF.Core.Models.ArticleData> CreateTestArticle()
    {
        string articleSuffix = Guid.NewGuid().ToString("N");

        return ArticleService.CreateArticle(
            $"TAF Article {articleSuffix}",
            "Article created by API automation.",
            "Initial article body.",
            new List<string> { "taf", "API" });
    }
}
