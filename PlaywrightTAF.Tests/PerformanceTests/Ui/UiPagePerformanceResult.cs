namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

internal sealed record UiPagePerformanceResult(
    string PageName,
    string Url,
    double PageReadyMs,
    double NavigationDurationMs,
    double DomContentLoadedMs,
    double BrowserLoadEventMs,
    double TransferSizeBytes,
    double EncodedBodySizeBytes);
