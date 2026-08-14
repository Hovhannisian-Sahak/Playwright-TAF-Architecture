using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.PimConfigurationPages.Base;

namespace PlaywrightTAF.UI.PimConfigurationPages;

public class DataImportPage : PimConfigurationBasePage
{
    public DataImportPage(IPage page) : base(page)
    {
    }
    private ILocator DownloadButton => Page.GetByText("Download");
    protected override string PageUrl => BuildUrl(ConfigurationReader.Current.BaseUrl, "/web/index.php/pim/pimCsvImport");

    public override Task<bool> IsLoadedAsync()
    {
        return Task.FromResult(CurrentUrl.Contains("pimCsvImport", StringComparison.OrdinalIgnoreCase));
    }

    public async Task DownloadDataImportFile()
    {
        var downloadTask = Page.WaitForDownloadAsync();

        await DownloadButton.ClickAsync();

        var download = await downloadTask;

        var path = Path.Combine(
            @"C:\Temp",
            download.SuggestedFilename);

        await download.SaveAsAsync(path);
    }
}
