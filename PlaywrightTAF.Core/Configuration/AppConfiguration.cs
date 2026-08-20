using PlaywrightTAF.Core.Authentication;

namespace PlaywrightTAF.Core.Configuration;

public sealed class AppConfiguration
{
    public string BaseUrl { get; init; } = "https://opensource-demo.orangehrmlive.com/";

    public string ApiBaseUrl { get; init; } = "https://conduit-api.bondaracademy.com";

    public string Browser { get; init; } = "chromium";

    public bool Headless { get; init; } = true;

    public int DefaultTimeoutMilliseconds { get; init; } = 30000;

    public Credentials Admin { get; init; } = new Credentials
    {
        Username = string.Empty,
        Password = string.Empty
    };

    public Credentials User { get; init; } = new Credentials
    {
        Username = string.Empty,
        Password = string.Empty
    };
}
