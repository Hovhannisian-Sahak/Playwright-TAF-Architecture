using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.AdminPages;

namespace PlaywrightTAF.Tests.PerformanceTests.Ui;

[TestFixture]
public sealed class UiPagePerformanceTests : AdminTest
{
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
        var result = await UiPagePerformanceMeasurer.MeasureAsync(pageName, page);

        AddAllureResultsAttachment(result, thresholds);
        AssertPerformanceThresholds(result, thresholds);
    }

    private static void AssertPerformanceThresholds(
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

    private static void AddAllureResultsAttachment(
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
