using System;
using System.IO;
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
        string filePath = Path.Combine(AppContext.BaseDirectory, "test.png");
        await adminCorporateBrandingPage.OpenAdminPageAsync();
        await adminCorporateBrandingPage.ClickToOpenCorporateBrandingAsync();
        await adminCorporateBrandingPage.ResetToDefaultAsync();
        await adminCorporateBrandingPage.ChooseColorAsync();
        await adminCorporateBrandingPage.ChooseClientLogoAsync(filePath);
        await adminCorporateBrandingPage.ClickPublishAsync();
    }
}
