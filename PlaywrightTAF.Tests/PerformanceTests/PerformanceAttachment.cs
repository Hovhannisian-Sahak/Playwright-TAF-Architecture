using System.Text;
using System.Text.Json;
using Allure.Net.Commons;

namespace PlaywrightTAF.Tests.PerformanceTests;

internal static class PerformanceAttachment
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static void AddJson(string name, object value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);

        AllureApi.AddAttachment(
            name,
            "application/json",
            Encoding.UTF8.GetBytes(json),
            ".json");
    }
}
