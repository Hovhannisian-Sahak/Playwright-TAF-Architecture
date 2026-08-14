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
        await PimConfigurationPage.OpenPimAsync();
        await PimConfigurationPage.OpenConfigurationMenuAsync();
        await PimConfigurationPage.OpenDataImportAsync();
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
        var downloadedFilePath = await DataImportPage.DownloadDataImportFileAsync(TestContext.CurrentContext.WorkDirectory);

        Assert.That(await DataImportPage.IsLoadedAsync(), Is.True);
        Assert.That(System.IO.File.Exists(downloadedFilePath), Is.True, "Downloaded file does not exist.");
    }
}
