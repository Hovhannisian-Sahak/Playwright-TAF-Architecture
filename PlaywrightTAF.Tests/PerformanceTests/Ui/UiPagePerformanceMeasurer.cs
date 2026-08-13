using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

internal static class UiPagePerformanceMeasurer
{
    public static async Task<UiPagePerformanceResult> MeasureAsync(string pageName, BasePage page)
    {
        await ClearBrowserResourceTimingsAsync(page);

        var pageReadyMs = await MeasurePageReadyAsync(pageName, page);
        var navigationTiming = await ReadNavigationTimingAsync(page);

        return new UiPagePerformanceResult(
            pageName,
            page.CurrentUrl,
            pageReadyMs,
            navigationTiming.DurationMs,
            navigationTiming.DomContentLoadedMs,
            navigationTiming.LoadEventMs,
            navigationTiming.TransferSizeBytes,
            navigationTiming.EncodedBodySizeBytes);
    }

    private static Task ClearBrowserResourceTimingsAsync(BasePage page)
    {
        return page.EvaluateAsync("() => performance.clearResourceTimings()");
    }

    private static async Task<double> MeasurePageReadyAsync(string pageName, BasePage page)
    {
        var stopwatch = Stopwatch.StartNew();
        await page.OpenAsync();
        bool isLoaded = await page.IsLoadedAsync();
        stopwatch.Stop();

        Assert.That(isLoaded, Is.True, $"{pageName} did not reach its loaded state.");

        return stopwatch.Elapsed.TotalMilliseconds;
    }

    private static Task<NavigationTiming> ReadNavigationTimingAsync(BasePage page)
    {
        return page.EvaluateAsync<NavigationTiming>(
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
    }
}
