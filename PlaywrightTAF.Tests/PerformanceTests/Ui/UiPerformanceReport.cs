namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

internal static class UiPerformanceReport
{
    public static void AttachResults(
        UiPagePerformanceResult result,
        UiPerformanceThresholds thresholds)
    {
        PerformanceAttachment.AddJson(
            $"{UiPerformanceThresholds.ToEnvironmentName(result.PageName).ToLowerInvariant()}-ui-performance-results",
            new
            {
                thresholds,
                result
            });
    }
}
