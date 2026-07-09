using Microsoft.Extensions.Configuration;

namespace PlaywrightTAF.Core.Configuration;

public static class ConfigurationReader
{
    public static AppConfiguration Current => Load();

    public static AppConfiguration Load(string? basePath = null)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath ?? AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables("TAF_")
            .Build();

        var config= new AppConfiguration
        {
            BaseUrl = GetString(configuration, "BaseUrl", "https://opensource-demo.orangehrmlive.com/"),
            ApiBaseUrl = GetString(configuration, "ApiBaseUrl", "https://example.com"),
            Browser = GetString(configuration, "Browser", "chromium"),
            Headless = GetBool(configuration, "Headless", false),
            DefaultTimeoutMilliseconds = GetInt(configuration, "DefaultTimeoutMilliseconds", 30000)
        };
        Validate(config);
        return config;
    }
    private static void Validate(AppConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.BaseUrl))
            throw new InvalidOperationException("BaseUrl is required.");

        if (!Uri.IsWellFormedUriString(config.BaseUrl, UriKind.Absolute))
            throw new InvalidOperationException($"Invalid BaseUrl: {config.BaseUrl}");

        if (config.DefaultTimeoutMilliseconds <= 0)
            throw new InvalidOperationException("DefaultTimeoutMilliseconds must be greater than 0.");

        string[] supportedBrowsers = { "chromium", "firefox", "webkit" };

        if (!supportedBrowsers.Contains(config.Browser.ToLowerInvariant()))
            throw new InvalidOperationException(
                $"Unsupported browser '{config.Browser}'. Supported values: chromium, firefox, webkit.");
    }
    private static string GetString(IConfiguration configuration, string key, string defaultValue)
    {
        string? value = configuration[key];
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    private static bool GetBool(IConfiguration configuration, string key, bool defaultValue)
    {
        string? value = configuration[key];
        return bool.TryParse(value, out bool parsedValue) ? parsedValue : defaultValue;
    }

    private static int GetInt(IConfiguration configuration, string key, int defaultValue)
    {
        string? value = configuration[key];
        return int.TryParse(value, out int parsedValue) ? parsedValue : defaultValue;
    }
}
