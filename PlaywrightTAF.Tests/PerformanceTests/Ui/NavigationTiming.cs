namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

internal sealed class NavigationTiming
{
    public double DurationMs { get; init; }

    public double DomContentLoadedMs { get; init; }

    public double LoadEventMs { get; init; }

    public double TransferSizeBytes { get; init; }

    public double EncodedBodySizeBytes { get; init; }
}
