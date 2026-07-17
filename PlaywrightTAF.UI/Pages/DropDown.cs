using Microsoft.Playwright;

namespace PlaywrightTAF.UI.Pages;

public class Dropdown
{
    private readonly IPage page;

    public Dropdown(IPage page)
    {
        this.page = page;
    }


    public async Task Select(
        string dropdown,
        string option)
    {
        await page
            .Locator(dropdown)
            .ClickAsync();


        await page
            .GetByText(option, new()
            {
                Exact=true
            })
            .ClickAsync();
    }
}