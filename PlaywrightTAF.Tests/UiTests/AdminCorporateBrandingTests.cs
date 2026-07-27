using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages.AdminPages;

namespace PlaywrightTAF.Tests.Tests;

public class AdminCorporateBrandingTests : AdminTest
{
    private AdminCorporateBrandingPage adminCorporateBrandingPage = null!;
    [SetUp]
    public void Setup()
    {
        adminCorporateBrandingPage = new AdminCorporateBrandingPage(Page);
    }
    
    [Test]
    public async Task AdminCanOpenCorporateBrandingPage()
    {
        await adminCorporateBrandingPage.OpenAdminPageAsync();
        await adminCorporateBrandingPage.ClickToOpenCorporateBrandingAsync();

        await adminCorporateBrandingPage.ChooseColorAsync();
        await adminCorporateBrandingPage.ClickPublishAsync();
    }
}