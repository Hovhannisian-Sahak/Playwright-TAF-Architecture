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
        return ArticleTestDataBuilder
            .New()
            .WithTitlePrefix(titlePrefix)
            .Build();
    }
}

public sealed class ArticleTestDataBuilder
{
    private string _titlePrefix = "TAF Article";
    private string? _title;
    private string _description = "Article created by API automation.";
    private string _body = "Initial article body.";
    private List<string> _tags = ["taf", "API"];

    private ArticleTestDataBuilder()
    {
    }

    public static ArticleTestDataBuilder New()
    {
        return new ArticleTestDataBuilder();
    }

    public ArticleTestDataBuilder WithTitlePrefix(string titlePrefix)
    {
        _titlePrefix = titlePrefix;
        return this;
    }

    public ArticleTestDataBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ArticleTestDataBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public ArticleTestDataBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public ArticleTestDataBuilder WithTags(params string[] tags)
    {
        _tags = [.. tags];
        return this;
    }

    public ArticleTestData Build()
    {
        string title = _title ?? $"{_titlePrefix} {Guid.NewGuid():N}";

        return new ArticleTestData(
            title,
            _description,
            _body,
            [.. _tags]);
    }
}
