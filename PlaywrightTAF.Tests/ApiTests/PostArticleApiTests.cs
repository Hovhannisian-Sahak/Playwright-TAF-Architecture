using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

namespace PlaywrightTAF.Tests.ApiTests;

public class PostArticleApiTests : BaseApiTest
{
    [Test]
    [Category("API")]
    public async Task PostArticle_ShouldCreateArticle()
    {
        string suffix = Guid.NewGuid().ToString("N");
        string title = $"POST API Article {suffix}";
        string description = "Created by POST API test.";
        string body = "This article verifies POST /api/articles.";
        var tags = new List<string> { "taf", "post" };
        string? createdSlug = null;

        try
        {
            var article = await ArticleService.CreateArticle(title, description, body, tags);
            createdSlug = article.slug;

            Assert.Multiple(() =>
            {
                Assert.That(article.slug, Is.Not.Empty);
                Assert.That(article.title, Is.EqualTo(title));
                Assert.That(article.description, Is.EqualTo(description));
                Assert.That(article.body, Is.EqualTo(body));
                Assert.That(article.tagList, Is.EquivalentTo(tags));
                Assert.That(article.author.username, Is.EqualTo(TestUsername));
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
}
