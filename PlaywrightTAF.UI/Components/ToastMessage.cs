using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.UI.Components;

public sealed class ToastMessage
{
    private readonly IPage _page;

    public ToastMessage(IPage page)
    {
        _page = page;
    }

    public Task WaitForSuccessAsync()
    {
        return WaitForTextAsync("Success");
    }

    public Task WaitForSavedAsync()
    {
        return WaitForTextAsync("Successfully Saved");
    }

    public Task WaitForUpdatedAsync()
    {
        return WaitForTextAsync("Successfully Updated");
    }

    public Task WaitForDeletedAsync()
    {
        return WaitForTextAsync("Successfully Deleted");
    }

    private Task WaitForTextAsync(string text)
    {
        return _page
            .GetByText(text, new() { Exact = true })
            .WaitForAsync(new()
            {
                Timeout = ConfigurationReader.Current.DefaultTimeoutMilliseconds
            });
    }
}
