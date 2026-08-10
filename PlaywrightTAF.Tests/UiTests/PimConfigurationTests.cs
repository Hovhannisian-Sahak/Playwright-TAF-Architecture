using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;
using PlaywrightTAF.UI.Pages.Base;

namespace PlaywrightTAF.Tests.UiTests;

public class PimConfigurationTests : AdminTest
{
    private readonly DataImportPage dataImportPage;
    private readonly PimConfigurationBasePage pimConfigurationPage;

    public PimConfigurationTests()
    {
        dataImportPage = PageObject<DataImportPage>();
        pimConfigurationPage = PageObject<PimConfigurationBasePage>();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDataImportPage()
    {
        await pimConfigurationPage.WaitAndClickPimButton();
        await pimConfigurationPage.WaitAndClickConfigurationButton();
        await pimConfigurationPage.WaitAndClickDataImportButton();
        Assert.That(await dataImportPage.IsLoadedAsync(), Is.True);
    }
    
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDataImportPageDirectly()
    {
        await dataImportPage.OpenAsync();
        Assert.That(await dataImportPage.IsLoadedAsync(), Is.True);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanDownloadFile()
    {
        await dataImportPage.OpenAsync();
        await dataImportPage.DownloadDataImportFile();
        Assert.That(await dataImportPage.IsLoadedAsync(), Is.True);
        Assert.That(System.IO.File.Exists(@"C:\Temp\importData.csv"), Is.True, "Downloaded file does not exist.");
    }
}
