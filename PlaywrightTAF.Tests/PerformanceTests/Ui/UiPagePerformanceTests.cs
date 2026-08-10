using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Allure.Net.Commons;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.AdminPages;

namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

[TestFixture]
public sealed class UiPagePerformanceTests : AdminTest
{
    private const double DefaultMaxPageReadyMs = 30000;
    private const double DefaultMaxBrowserLoadMs = 10000;
    private AdminCorporateBrandingPage AdminCorporateBrandingPage => PageObject<AdminCorporateBrandingPage>();
    private DashboardPage DashboardPage => PageObject<DashboardPage>();

    [Test]
    [Category("Performance")]
    [Category("UIPerformance")]
    public async Task Dashboard_ShouldMeetPerformanceThresholds()
    {
        await AssertPageMeetsPerformanceThresholdsAsync(
            "Dashboard",
            DashboardPage);
    }

    [Test]
    [Category("Performance")]
    [Category("UIPerformance")]
    public async Task CorporateBranding_ShouldMeetPerformanceThresholds()
    {
        await AssertPageMeetsPerformanceThresholdsAsync(
            "Corporate Branding",
            AdminCorporateBrandingPage);
    }

    private async Task AssertPageMeetsPerformanceThresholdsAsync(string pageName, BasePage page)
    {
        var thresholds = UiPerformanceThresholds.FromEnvironment(pageName);
        var result = await MeasurePageAsync(pageName, page);

        AddAllureResultsAttachment(result, thresholds);

        Assert.Multiple(() =>
        {
            Assert.That(
                result.PageReadyMs,
                Is.LessThanOrEqualTo(thresholds.MaxPageReadyMs),
                $"{result.PageName} should become ready within {thresholds.MaxPageReadyMs:N0} ms.");

            Assert.That(
                result.BrowserLoadMs,
                Is.LessThanOrEqualTo(thresholds.MaxBrowserLoadMs),
                $"{result.PageName} browser load event should complete within {thresholds.MaxBrowserLoadMs:N0} ms.");
        });
    }

    private async Task<UiPagePerformanceResult> MeasurePageAsync(string pageName, BasePage page)
    {
        await page.EvaluateAsync("() => performance.clearResourceTimings()");

        var stopwatch = Stopwatch.StartNew();
        await page.OpenAsync();
        bool isLoaded = await page.IsLoadedAsync();
        stopwatch.Stop();

        Assert.That(isLoaded, Is.True, $"{pageName} did not reach its loaded state.");

        var navigationTiming = await page.EvaluateAsync<NavigationTiming>(
            """
            () => {
                const navigation = performance.getEntriesByType('navigation').at(-1);

                return {
                    durationMs: navigation?.duration ?? 0,
                    domContentLoadedMs: navigation
                        ? navigation.domContentLoadedEventEnd - navigation.startTime
                        : 0,
                    loadEventMs: navigation
                        ? navigation.loadEventEnd - navigation.startTime
                        : 0,
                    transferSizeBytes: navigation?.transferSize ?? 0,
                    encodedBodySizeBytes: navigation?.encodedBodySize ?? 0
                };
            }
            """);

        return new UiPagePerformanceResult(
            pageName,
            page.CurrentUrl,
            stopwatch.Elapsed.TotalMilliseconds,
            navigationTiming.DurationMs,
            navigationTiming.DomContentLoadedMs,
            navigationTiming.LoadEventMs,
            navigationTiming.TransferSizeBytes,
            navigationTiming.EncodedBodySizeBytes);
    }

    private static void AddAllureResultsAttachment(
        UiPagePerformanceResult result,
        UiPerformanceThresholds thresholds)
    {
        var json = JsonSerializer.Serialize(
            new
            {
                thresholds,
                result
            },
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        AllureApi.AddAttachment(
            $"{ToEnvironmentName(result.PageName).ToLowerInvariant()}-ui-performance-results",
            "application/json",
            Encoding.UTF8.GetBytes(json),
            ".json");
    }

    private sealed record UiPerformanceThresholds(
        double MaxPageReadyMs,
        double MaxBrowserLoadMs)
    {
        public static UiPerformanceThresholds FromEnvironment(string pageName)
        {
            string pagePrefix = ToEnvironmentName(pageName);

            return new UiPerformanceThresholds(
                GetDouble(
                    $"UI_PERF_{pagePrefix}_MAX_PAGE_READY_MS",
                    GetDouble("UI_PERF_MAX_PAGE_READY_MS", DefaultMaxPageReadyMs)),
                GetDouble(
                    $"UI_PERF_{pagePrefix}_MAX_BROWSER_LOAD_MS",
                    GetDouble("UI_PERF_MAX_BROWSER_LOAD_MS", DefaultMaxBrowserLoadMs)));
        }

        private static double GetDouble(string name, double defaultValue)
        {
            string? value = Environment.GetEnvironmentVariable(name);

            return double.TryParse(value, out double parsedValue)
                ? parsedValue
                : defaultValue;
        }
    }

    private static string ToEnvironmentName(string value)
    {
        return value
            .Replace(" ", "_", StringComparison.OrdinalIgnoreCase)
            .ToUpperInvariant();
    }

    private sealed record UiPagePerformanceResult(
        string PageName,
        string Url,
        double PageReadyMs,
        double NavigationDurationMs,
        double DomContentLoadedMs,
        double BrowserLoadMs,
        double TransferSizeBytes,
        double EncodedBodySizeBytes);

    private sealed class NavigationTiming
    {
        public double DurationMs { get; init; }

        public double DomContentLoadedMs { get; init; }

        public double LoadEventMs { get; init; }

        public double TransferSizeBytes { get; init; }

        public double EncodedBodySizeBytes { get; init; }
    }
}
