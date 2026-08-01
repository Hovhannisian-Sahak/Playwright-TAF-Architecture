using System.Globalization;
using NUnit.Framework;
using Performance;

namespace PlaywrightTAF.Tests.PerformanceTests;

[TestFixture]
public sealed class PerformanceOptionsTests
{
    [Test]
    public void FromArgs_ParsesDecimalOptionsUsingInvariantCulture()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        var originalUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var culture = CultureInfo.GetCultureInfo("ru-RU");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var options = PerformanceOptions.FromArgs(
            [
                "--max-failure-rate",
                "0.01",
                "--max-p95-ms",
                "1000.5"
            ]);

            Assert.Multiple(() =>
            {
                Assert.That(options.MaxFailureRate, Is.EqualTo(0.01));
                Assert.That(options.MaxP95Ms, Is.EqualTo(1000.5));
            });
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
            CultureInfo.CurrentUICulture = originalUiCulture;
        }
    }
}
