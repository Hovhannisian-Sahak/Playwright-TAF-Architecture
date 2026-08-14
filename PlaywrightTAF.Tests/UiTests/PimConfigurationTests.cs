using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.PimConfigurationPages;
using PlaywrightTAF.UI.PimConfigurationPages.Base;

namespace PlaywrightTAF.Tests.UiTests;

public class PimConfigurationTests : AdminTest
{
    private DataImportPage DataImportPage => PageObject<DataImportPage>();
    private PimConfigurationBasePage PimConfigurationPage => PageObject<PimConfigurationBasePage>();

    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDataImportPage()
    {
        await PimConfigurationPage.WaitAndClickPimButton();
        await PimConfigurationPage.WaitAndClickConfigurationButton();
        await PimConfigurationPage.WaitAndClickDataImportButton();
        Assert.That(await DataImportPage.IsLoadedAsync(), Is.True);
    }
    
    [Test]
    [Category("UI")]
    public async Task AdminCanOpenDataImportPageDirectly()
    {
        await DataImportPage.OpenAsync();
        Assert.That(await DataImportPage.IsLoadedAsync(), Is.True);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanDownloadFile()
    {
        await DataImportPage.OpenAsync();
        await DataImportPage.DownloadDataImportFile();
        Assert.That(await DataImportPage.IsLoadedAsync(), Is.True);
        Assert.That(System.IO.File.Exists(@"C:\Temp\importData.csv"), Is.True, "Downloaded file does not exist.");
    }
}
