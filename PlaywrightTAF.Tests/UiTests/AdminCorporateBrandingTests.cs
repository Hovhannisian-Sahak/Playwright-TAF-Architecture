using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.Tests.TestData;
using PlaywrightTAF.UI.Pages.AdminPages;

namespace PlaywrightTAF.Tests.UiTests;

public class AdminCorporateBrandingTests : AdminTest
{
    private AdminCorporateBrandingPage adminCorporateBrandingPage = null!;
    [SetUp]
    public void Setup()
    {
        adminCorporateBrandingPage = new AdminCorporateBrandingPage(Page);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenCorporateBrandingPage()
    {
        string filePath = TestDataFactory.UploadFilePath();

        await adminCorporateBrandingPage.OpenAdminPageAsync();
        await adminCorporateBrandingPage.ClickToOpenCorporateBrandingAsync();
        await adminCorporateBrandingPage.ResetToDefaultAsync();
        await adminCorporateBrandingPage.ChooseColorAsync();
        await adminCorporateBrandingPage.ChooseClientLogoAsync(filePath);
        await adminCorporateBrandingPage.ClickPublishAsync();
    }
}
