using Microsoft.Extensions.Configuration;

namespace PlaywrightTAF.Core.Configuration;

public static class ConfigurationReader
{
    private static readonly Lazy<AppConfiguration> CachedConfiguration = new(() => Load());

    public static AppConfiguration Current => CachedConfiguration.Value;

    public static AppConfiguration Load(string? basePath = null)
    {
        var defaults = new AppConfiguration();

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath ?? AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables("TAF_")
            .Build();

        var config = new AppConfiguration
        {
            BaseUrl = GetString(configuration, "BaseUrl", defaults.BaseUrl),
            ApiBaseUrl = GetString(configuration, "ApiBaseUrl", defaults.ApiBaseUrl),
            Browser = GetString(configuration, "Browser", defaults.Browser),
            Headless = GetBool(configuration, "Headless", defaults.Headless),
            DefaultTimeoutMilliseconds = GetInt(configuration, "DefaultTimeoutMilliseconds", defaults.DefaultTimeoutMilliseconds),
            Admin = GetCredentials(configuration, "Admin", defaults.Admin),
            User = GetCredentials(configuration, "User", defaults.User)
        };

        Validate(config);
        return config;
    }

    private static void Validate(AppConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config.BaseUrl))
        {
            throw new InvalidOperationException("BaseUrl is required.");
        }

        if (!Uri.IsWellFormedUriString(config.BaseUrl, UriKind.Absolute))
        {
            throw new InvalidOperationException($"Invalid BaseUrl: {config.BaseUrl}");
        }

        if (string.IsNullOrWhiteSpace(config.ApiBaseUrl))
        {
            throw new InvalidOperationException("ApiBaseUrl is required.");
        }

        if (!Uri.IsWellFormedUriString(config.ApiBaseUrl, UriKind.Absolute))
        {
            throw new InvalidOperationException($"Invalid ApiBaseUrl: {config.ApiBaseUrl}");
        }

        if (config.DefaultTimeoutMilliseconds <= 0)
        {
            throw new InvalidOperationException("DefaultTimeoutMilliseconds must be greater than 0.");
        }

        string[] supportedBrowsers = { "chromium", "firefox", "webkit" };

        if (!supportedBrowsers.Contains(config.Browser.ToLowerInvariant()))
        {
            throw new InvalidOperationException($"Unsupported browser '{config.Browser}'. Supported values: chromium, firefox, webkit.");
        }

        ValidateCredentials(config.Admin, "Admin");
        ValidateCredentials(config.User, "User");
    }

    private static string GetString(IConfiguration configuration, string key, string defaultValue)
    {
        string? value = configuration[key];
        return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
    }

    private static bool GetBool(IConfiguration configuration, string key, bool defaultValue)
    {
        string? value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (bool.TryParse(value, out bool parsedValue))
        {
            return parsedValue;
        }

        throw new InvalidOperationException($"Configuration value '{key}' must be 'true' or 'false'. Actual value: '{value}'.");
    }

    private static int GetInt(IConfiguration configuration, string key, int defaultValue)
    {
        string? value = configuration[key];

        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (int.TryParse(value, out int parsedValue))
        {
            return parsedValue;
        }

        throw new InvalidOperationException($"Configuration value '{key}' must be an integer. Actual value: '{value}'.");
    }

    private static Authentication.Credentials GetCredentials(IConfiguration configuration, string sectionName, Authentication.Credentials defaultCredentials)
    {
        IConfigurationSection section = configuration.GetSection(sectionName);

        return new Authentication.Credentials
        {
            Username = GetString(section, "Username", defaultCredentials.Username),
            Password = GetString(section, "Password", defaultCredentials.Password)
        };
    }

    private static void ValidateCredentials(Authentication.Credentials credentials, string sectionName)
    {
        if (string.IsNullOrWhiteSpace(credentials.Username))
        {
            throw new InvalidOperationException($"{sectionName}:Username is required.");
        }

        if (string.IsNullOrWhiteSpace(credentials.Password))
        {
            throw new InvalidOperationException($"{sectionName}:Password is required.");
        }
    }
}
