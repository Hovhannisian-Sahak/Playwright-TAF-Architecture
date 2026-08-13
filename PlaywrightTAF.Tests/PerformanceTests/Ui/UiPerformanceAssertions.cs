using NUnit.Framework;

namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

internal static class UiPerformanceAssertions
{
    public static void ShouldMeetThresholds(
        UiPagePerformanceResult result,
        UiPerformanceThresholds thresholds)
    {
        Assert.Multiple(() =>
        {
            Assert.That(
                result.PageReadyMs,
                Is.LessThanOrEqualTo(thresholds.MaxPageReadyMs),
                $"{result.PageName} should become ready within {thresholds.MaxPageReadyMs:N0} ms.");

            Assert.That(
                result.BrowserLoadEventMs,
                Is.LessThanOrEqualTo(thresholds.MaxBrowserLoadMs),
                $"{result.PageName} browser load event should complete within {thresholds.MaxBrowserLoadMs:N0} ms.");
        });
    }
}
