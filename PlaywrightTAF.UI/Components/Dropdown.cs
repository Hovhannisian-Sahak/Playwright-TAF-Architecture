using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Components;

public class Dropdown
{
    private readonly IPage _page;

    public Dropdown(IPage page)
    {
        _page = page;
    }

    public async Task Select(string dropdown, string option)
    {
        await _page
            .Locator(dropdown)
            .ClickAsync();

        await _page
            .GetByText(option, new()
            {
                Exact = true
            })
            .ClickAsync();
    }
}
