using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Logging;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.Tests.TestData;
using PlaywrightTAF.UI.Pages.AdminPages;
using Serilog;

namespace PlaywrightTAF.Tests.UiTests;

public class AdminCorporateBrandingTests : AdminTest
{
    private static readonly ILogger Logger = LogProvider.ForContext<AdminCorporateBrandingTests>();

    private AdminCorporateBrandingPage AdminCorporateBrandingPage => PageObject<AdminCorporateBrandingPage>();

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenCorporateBrandingPage()
    {
        string filePath = TestDataFactory.UploadFilePath();

        try
        {
            await AdminCorporateBrandingPage.OpenAdminPageAsync();
            await AdminCorporateBrandingPage.ClickToOpenCorporateBrandingAsync();
            await AdminCorporateBrandingPage.ResetToDefaultAsync();
            await AdminCorporateBrandingPage.ChangeSecondaryFontColorAsync();
            await AdminCorporateBrandingPage.UploadClientLogoAsync(filePath);
            await AdminCorporateBrandingPage.PublishAsync();
            await AdminCorporateBrandingPage.ExpectSuccessfullySavedAsync();
        }
        finally
        {
            await RestoreCorporateBrandingDefaultsAsync();
        }
    }

    private async Task RestoreCorporateBrandingDefaultsAsync()
    {
        try
        {
            await AdminCorporateBrandingPage.RestoreDefaultsAsync();
        }
        catch (System.Exception ex)
        {
            Logger.Warning(ex, "Could not restore corporate branding defaults after UI test.");
        }
    }
}
