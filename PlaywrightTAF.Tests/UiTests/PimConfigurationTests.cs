using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.Base;

namespace PlaywrightTAF.Tests.UiTests;

public class PimConfigurationTests : AdminTest
{
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDataImportPage()
    {
        var pimConfigurationPage = new PimConfigurationBasePage(Page);
        await pimConfigurationPage.WaitAndClickPimButton();
        await pimConfigurationPage.WaitAndClickConfigurationButton();
        await pimConfigurationPage.WaitAndClickDataImportButton();
        var dataImportPage = new DataImportPage(Page);
        Assert.That(await dataImportPage.IsLoadedAsync(), Is.True);
    }
    
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDataImportPageDirectly()
    {
        var dataImportPage = new DataImportPage(Page);
        await dataImportPage.OpenAsync();
        Assert.That(await dataImportPage.IsLoadedAsync(), Is.True);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanDownloadFile()
    {
        var dataImportPage = new DataImportPage(Page);
        await dataImportPage.OpenAsync();
        await dataImportPage.DownloadDataImportFile();
        Assert.That(await dataImportPage.IsLoadedAsync(), Is.True);
        Assert.That(System.IO.File.Exists(@"C:\Temp\importData.csv"), Is.True, "Downloaded file does not exist.");
    }
}