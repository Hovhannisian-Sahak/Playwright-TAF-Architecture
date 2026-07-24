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

    private ILocator MyInfoLink => Page.GetByRole(AriaRole.Link, new() { Name = "My Info" });
    private ILocator PersonalDetailsHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Personal Details" });
    private ILocator LastNameInput => Page.Locator("input[name='lastName']");
    private ILocator Dropdowns => Page.Locator(".oxd-select-wrapper");
    private ILocator DateInputs => Page.Locator(".oxd-date-input input");
    private ILocator DatePickerIcons => Page.Locator(".oxd-date-input i");
    private ILocator CalendarYearSelector => Page.Locator(".oxd-calendar-selector-year-selected > .oxd-icon");
    private ILocator CalendarMonthSelector => Page.Locator(".oxd-calendar-selector-month-selected > .oxd-icon");
    private ILocator CalendarDates => Page.Locator(".oxd-calendar-date");
    private ILocator CalendarMenu => Page.GetByRole(AriaRole.Menu);
    private ILocator SaveButtons => Page.GetByRole(AriaRole.Button, new() { Name = "Save" });
    private ILocator SuccessfullyUpdatedText => Page.GetByText("Successfully Updated", new() { Exact = true });
    private ILocator AddAttachmentButton => Page.GetByRole(AriaRole.Button, new() { Name = "Add" });
    private ILocator AttachmentCard => Page.Locator(".orangehrm-card-container").Nth(2);
    private ILocator FileButton => Page.Locator(".oxd-file-button");
    private ILocator FileInput => Page.Locator(".oxd-file-input-div");
    private ILocator CommentInput => Page.GetByPlaceholder("Type comment here");
    private ILocator SuccessfullySavedText => Page.GetByText("Successfully Saved", new() { Exact = true });
    private ILocator Listbox => Page.GetByRole(AriaRole.Listbox);

    public async Task OpenPersonalDetailsAsync()
    {
        await MyInfoLink.ClickAsync();
        await Expect(PersonalDetailsHeading).ToBeVisibleAsync();
    }

    public override Task<bool> IsLoadedAsync()
    {
        return PersonalDetailsHeading.IsVisibleAsync();
    }

    public async Task FillLastNameAsync(string lastName)
    {
        await LastNameInput.ClearAsync();
        await LastNameInput.FillAsync(lastName);
        await Expect(LastNameInput).ToHaveValueAsync(lastName);
    }

    public async Task SelectNationalityAsync(string nationality)
    {
        await SelectDropdownOptionAsync(0, nationality);
        await Expect(Dropdowns.Nth(0)).ToContainTextAsync(nationality);
    }

    public async Task SetBirthDateAsync()
    {
        const string expectedBirthDate = "2025-19-11";
        var birthDateInput = DateInputs.Nth(1);

        await DatePickerIcons.Nth(1).ClickAsync();
        await CalendarYearSelector.ClickAsync();
        await CalendarMenu.GetByText("2025", new() { Exact = true }).ClickAsync();

        await CalendarMonthSelector.ClickAsync();
        await CalendarMenu.GetByText("November", new() { Exact = true }).ClickAsync();

        await CalendarDates.GetByText("19", new() { Exact = true }).ClickAsync();

        await Expect(birthDateInput).ToHaveValueAsync(expectedBirthDate);
    }

    public async Task SavePersonalDetailsAsync()
    {
        await SaveButtons.First.ClickAsync();
        await SuccessfullyUpdatedText.WaitForAsync();
    }

    public async Task OpenAttachmentFormAsync()
    {
        await AddAttachmentButton.ClickAsync();
        await AttachmentCard.WaitForAsync(new()
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
        await FileButton.ClickAsync();
        var chooser = await fileChooserTask;

        await chooser.SetFilesAsync(filePath);

        await Expect(FileInput).ToContainTextAsync(Path.GetFileName(filePath));

        await CommentInput.FillAsync(comment);
        await Expect(CommentInput).ToHaveValueAsync(comment);

        await SaveButtons.Nth(2).ClickAsync();
        await SuccessfullySavedText.WaitForAsync();
    }

    private async Task SelectDropdownOptionAsync(int dropdownIndex, string option)
    {
        await Dropdowns.Nth(dropdownIndex).ClickAsync();

        await Listbox
            .GetByText(option, new() { Exact = true })
            .ClickAsync();
    }
}
