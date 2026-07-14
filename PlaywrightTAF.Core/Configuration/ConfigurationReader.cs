using Microsoft.Extensions.Configuration;

namespace PlaywrightTAF.Core.Configuration;

public static class ConfigurationReader
{
    private static readonly Lazy<AppConfiguration> CachedConfiguration = new(() => Load());

    public static AppConfiguration Current => CachedConfiguration.Value;

    public static AppConfiguration Load(string? basePath = null)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(basePath ?? AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables("TAF_")
            .Build();

        var config = new AppConfiguration
        {
            BaseUrl = GetString(configuration, "BaseUrl", "https://opensource-demo.orangehrmlive.com/"),
            ApiBaseUrl = GetString(configuration, "ApiBaseUrl", "https://conduit-api.bondaracademy.com"),
            Browser = GetString(configuration, "Browser", "chromium"),
            Headless = GetBool(configuration, "Headless", false),
            DefaultTimeoutMilliseconds = GetInt(configuration, "DefaultTimeoutMilliseconds", 30000),
            Admin = GetCredentials(configuration, "Admin", new Authentication.Credentials
            {
                Username = "Admin",
                Password = "admin123"
            }),
            User = GetCredentials(configuration, "User", new Authentication.Credentials
            {
                Username = "Users",
                Password = "users123"
            })
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
        return bool.TryParse(value, out bool parsedValue) ? parsedValue : defaultValue;
    }

    private static int GetInt(IConfiguration configuration, string key, int defaultValue)
    {
        string? value = configuration[key];
        return int.TryParse(value, out int parsedValue) ? parsedValue : defaultValue;
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
