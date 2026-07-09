namespace PlaywrightTAF.Core.Configuration;

public sealed class AppConfiguration
{
    public string BaseUrl { get; init; } = "https://opensource-demo.orangehrmlive.com/";

    public string ApiBaseUrl { get; init; } = "https://example.com";

    public string Browser { get; init; } = "chromium";

    public bool Headless { get; init; } = true;

    public int DefaultTimeoutMilliseconds { get; init; } = 30000;
}
