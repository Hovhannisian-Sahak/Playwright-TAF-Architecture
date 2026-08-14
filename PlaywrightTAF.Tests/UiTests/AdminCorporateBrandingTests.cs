using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.Tests.TestData;
using PlaywrightTAF.UI.Pages.AdminPages;

namespace PlaywrightTAF.Tests.UiTests;

public class AdminCorporateBrandingTests : AdminTest
{
    private AdminCorporateBrandingPage AdminCorporateBrandingPage => PageObject<AdminCorporateBrandingPage>();

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenCorporateBrandingPage()
    {
        string filePath = TestDataFactory.UploadFilePath();

        await AdminCorporateBrandingPage.OpenAdminPageAsync();
        await AdminCorporateBrandingPage.ClickToOpenCorporateBrandingAsync();
        await AdminCorporateBrandingPage.ResetToDefaultAsync();
        await AdminCorporateBrandingPage.ChangeSecondaryFontColorAsync();
        await AdminCorporateBrandingPage.UploadClientLogoAsync(filePath);
        await AdminCorporateBrandingPage.PublishAsync();
        await AdminCorporateBrandingPage.ExpectSuccessfullySavedAsync();
    }
}
