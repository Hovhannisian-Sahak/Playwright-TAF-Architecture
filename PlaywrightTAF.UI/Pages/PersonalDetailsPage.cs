using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages;

public class PersonalDetailsPage : BasePage
{
    public PersonalDetailsPage(IPage page) : base(page)
    {
    }

    protected override string PageUrl => ConfigurationReader.Current.BaseUrl;

    public async Task OpenPersonalDetailsAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Info" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Personal Details" })).ToBeVisibleAsync();
    }

    public override Task<bool> IsLoadedAsync()
    {
        return Page.GetByRole(AriaRole.Heading, new() { Name = "Personal Details" }).IsVisibleAsync();
    }

    public async Task FillLastNameAsync(string lastName)
    {
        var lastNameInput = Page.Locator("input[name='lastName']");

        await lastNameInput.ClearAsync();
        await lastNameInput.FillAsync(lastName);
        await Expect(lastNameInput).ToHaveValueAsync(lastName);
    }

    public async Task SelectNationalityAsync(string nationality)
    {
        await SelectDropdownOptionAsync(0, nationality);
        await Expect(Page.Locator(".oxd-select-wrapper").Nth(0)).ToContainTextAsync(nationality);
    }

    public async Task SetBirthDateAsync()
    {
        const string expectedBirthDate = "2025-19-11";
        var birthDateInput = Page.Locator(".oxd-date-input input").Nth(1);

        await Page.Locator(".oxd-date-input i").Nth(1).ClickAsync();
        await Page.Locator(".oxd-calendar-selector-year-selected > .oxd-icon").ClickAsync();
        await Page.GetByRole(AriaRole.Menu).GetByText("2025", new() { Exact = true }).ClickAsync();

        await Page.Locator(".oxd-calendar-selector-month-selected > .oxd-icon").ClickAsync();
        await Page.GetByRole(AriaRole.Menu).GetByText("November", new() { Exact = true }).ClickAsync();

        await Page.Locator(".oxd-calendar-date").GetByText("19", new() { Exact = true }).ClickAsync();

        await Expect(birthDateInput).ToHaveValueAsync(expectedBirthDate);
    }

    public async Task SavePersonalDetailsAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).First.ClickAsync();
        await Page.GetByText("Successfully Updated", new() { Exact = true }).WaitForAsync();
    }

    public async Task OpenAttachmentFormAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add" }).ClickAsync();
        await Page.Locator(".orangehrm-card-container").Nth(2).WaitForAsync(new()
        {
            State = WaitForSelectorState.Visible
        });
    }

    public async Task UploadFileAndMakeCommentAsync(string filePath, string comment)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Upload test file was not found: {filePath}", filePath);
        }

        var fileChooserTask = Page.WaitForFileChooserAsync();
        await Page.Locator(".oxd-file-button").ClickAsync();
        var chooser = await fileChooserTask;

        await chooser.SetFilesAsync(filePath);

        await Expect(Page.Locator(".oxd-file-input-div")).ToContainTextAsync(Path.GetFileName(filePath));

        var commentInput = Page.GetByPlaceholder("Type comment here");
        await commentInput.FillAsync(comment);
        await Expect(commentInput).ToHaveValueAsync(comment);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).Nth(2).ClickAsync();
        await Page.GetByText("Successfully Saved", new() { Exact = true }).WaitForAsync();
    }

    private async Task SelectDropdownOptionAsync(int dropdownIndex, string option)
    {
        await Page.Locator(".oxd-select-wrapper")
            .Nth(dropdownIndex)
            .ClickAsync();

        await Page.GetByRole(AriaRole.Listbox)
            .GetByText(option, new() { Exact = true })
            .ClickAsync();
    }
}
