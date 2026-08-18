using Microsoft.Playwright;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.UI.Components;
using static Microsoft.Playwright.Assertions;

namespace PlaywrightTAF.UI.Pages.UserManagementPages;

public class PersonalDetailsPage : BasePage
{
    private readonly ToastMessage _toastMessage;

    public PersonalDetailsPage(IPage page, ToastMessage toastMessage) : base(page)
    {
        _toastMessage = toastMessage;
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
    private ILocator AddAttachmentButton => Page.GetByRole(AriaRole.Button, new() { Name = "Add" });
    private ILocator AttachmentCard => Page.Locator(".orangehrm-card-container").Nth(2);
    private ILocator FileButton => Page.Locator(".oxd-file-button");
    private ILocator FileInput => Page.Locator(".oxd-file-input-div");
    private ILocator CommentInput => Page.GetByPlaceholder("Type comment here");
    private ILocator ConfirmDeleteButton => Page.Locator(".orangehrm-modal-footer")
        .Locator("button")
        .Nth(1);

    public async Task OpenPersonalDetailsAsync()
    {
        await MyInfoLink.ClickAsync();
        await WaitUntilVisibleAsync(PersonalDetailsHeading);
    }

    public override Task<bool> IsLoadedAsync()
    {
        return PersonalDetailsHeading.IsVisibleAsync();
    }

    public async Task FillLastNameAsync(string lastName)
    {
        await ClearFillAndExpectValueAsync(LastNameInput, lastName);
    }

    public Task<string> GetLastNameAsync()
    {
        return LastNameInput.InputValueAsync();
    }

    public async Task SelectNationalityAsync(string nationality)
    {
        await SelectDropdownOptionAsync(Dropdowns, 0, nationality);
    }

    public async Task ExpectNationalityAsync(string nationality)
    {
        await Expect(Dropdowns.Nth(0)).ToContainTextAsync(nationality);
    }

    public async Task<string> GetNationalityAsync()
    {
        return (await Dropdowns.Nth(0).InnerTextAsync()).Trim();
    }

    public async Task SetBirthDateAsync()
    {
        await DatePickerIcons.Nth(1).ClickAsync();
        await CalendarYearSelector.ClickAsync();
        await CalendarMenu.GetByText("2025", new() { Exact = true }).ClickAsync();

        await CalendarMonthSelector.ClickAsync();
        await CalendarMenu.GetByText("November", new() { Exact = true }).ClickAsync();

        await CalendarDates.GetByText("19", new() { Exact = true }).ClickAsync();
    }

    public async Task SetBirthDateValueAsync(string birthDate)
    {
        await ClearFillAndExpectValueAsync(DateInputs.Nth(1), birthDate);
    }

    public Task<string> GetBirthDateAsync()
    {
        return DateInputs.Nth(1).InputValueAsync();
    }

    public async Task ExpectBirthDateAsync()
    {
        const string expectedBirthDate = "2025-19-11";
        var birthDateInput = DateInputs.Nth(1);
        await Expect(birthDateInput).ToHaveValueAsync(expectedBirthDate);
    }

    public async Task SavePersonalDetailsAsync()
    {
        await SaveButtons.First.ClickAsync();
    }

    public async Task ExpectPersonalDetailsUpdatedAsync()
    {
        await _toastMessage.WaitForUpdatedAsync();
    }

    public async Task OpenAttachmentFormAsync()
    {
        await AddAttachmentButton.ClickAsync();
        await WaitUntilVisibleAsync(AttachmentCard);
    }

    public async Task UploadFileAndMakeCommentAsync(string filePath, string comment)
    {
        await UploadFileAsync(FileButton, FileInput, filePath);

        await FillAndExpectValueAsync(CommentInput, comment);

        await SaveButtons.Nth(2).ClickAsync();
    }

    public async Task ExpectAttachmentSavedAsync()
    {
        await _toastMessage.WaitForSavedAsync();
    }

    public async Task DeleteAttachmentByCommentAsync(string comment)
    {
        var attachmentRow = Page.Locator(".oxd-table-row")
            .Filter(new() { HasText = comment })
            .First;

        await WaitUntilVisibleAsync(attachmentRow);
        await attachmentRow
            .Locator(".oxd-table-cell-actions")
            .Locator("button")
            .First
            .ClickAsync();

        await ConfirmDeleteButton.ClickAsync();
        await _toastMessage.WaitForDeletedAsync();
    }
}
