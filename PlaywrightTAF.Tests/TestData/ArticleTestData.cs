using System;
using System.Collections.Generic;

namespace PlaywrightTAF.Tests.TestData;

public sealed record ArticleTestData(
    string Title,
    string Description,
    string Body,
    List<string> Tags)
{
    public static ArticleTestData Create(string titlePrefix = "TAF Article")
    {
        string articleSuffix = Guid.NewGuid().ToString("N");

        return new ArticleTestData(
            $"{titlePrefix} {articleSuffix}",
            "Article created by API automation.",
            "Initial article body.",
            ["taf", "API"]);
    }
}
