using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.Tests.TestData;
using PlaywrightTAF.UI.Pages.UserManagementPages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserManagementTests : AdminTest
{
    private const string EmployeeName = "Ranga  Akunuri";
    private const string UserPassword = "TestUser123!@#Aa";
    private const string ChangePassword = "TestUser123!@#Aab";

    private AddUserPage AddUserPage => PageObject<AddUserPage>();
    private DeleteUserPage DeleteUserPage => PageObject<DeleteUserPage>();
    private EditUserPage EditUserPage => PageObject<EditUserPage>();
    private PersonalDetailsPage PersonalDetailsPage => PageObject<PersonalDetailsPage>();

    [Test]
    [Category("UI")]
    public async Task AdminCanAddUser()
    {
        var newUsername = TestDataFactory.UniqueUsername("Adminn");

        await CreateAdminUserAndSearchAsync(newUsername);

        await AddUserPage.ExpectUserExistsAsync(newUsername);

        await DeleteUserPage.DeleteFirstSearchResultAsync();
        UntrackUserFromCleanup(newUsername);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanDeleteUser()
    {
        var newUsername = TestDataFactory.UniqueUsername("Adminn");

        await AddUserPage.OpenAddUserFormAsync();
        await AddUserPage.CreateAdminUserAsync(newUsername, EmployeeName, UserPassword);
        TrackUserForCleanup(newUsername);
        await DeleteUserPage.SearchUserAsync(newUsername);

        await DeleteUserPage.ExpectUserExistsAsync(newUsername);

        await DeleteUserPage.DeleteFirstSearchResultAsync();
        UntrackUserFromCleanup(newUsername);
        await DeleteUserPage.SearchUserAsync(newUsername);

        await DeleteUserPage.ExpectUserDoesNotExistAsync(newUsername);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanChangeUserNameAndPassword()
    {
        var newUsername = TestDataFactory.UniqueUsername("Adminn");
        var changedUsername = TestDataFactory.UniqueUsername("ChangedAdminn");

        await CreateAdminUserAndSearchAsync(newUsername);

        await EditUserPage.ExpectUserExistsAsync(newUsername);

        await EditUserPage.EditFirstSearchResultAsync(changedUsername, ChangePassword);
        UntrackUserFromCleanup(newUsername);
        TrackUserForCleanup(changedUsername);
        await EditUserPage.SearchUserAsync(changedUsername);

        await EditUserPage.ExpectUserExistsAsync(changedUsername);
        await DeleteUserPage.DeleteFirstSearchResultAsync();
        UntrackUserFromCleanup(changedUsername);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanEditInfo()
    {
        var lastName = TestDataFactory.UniqueUsername("Admin");
        string filePath = TestDataFactory.UploadFilePath();
        string attachmentComment = TestDataFactory.UniqueUsername("ProfileAttachment");

        await TrackPersonalDetailsForCleanupAsync();
        TrackPersonalAttachmentForCleanup(attachmentComment);
        await PersonalDetailsPage.OpenPersonalDetailsAsync();
        await PersonalDetailsPage.FillLastNameAsync(lastName);
        await PersonalDetailsPage.SelectNationalityAsync("Armenian");
        await PersonalDetailsPage.ExpectNationalityAsync("Armenian");
        await PersonalDetailsPage.SetBirthDateAsync();
        await PersonalDetailsPage.ExpectBirthDateAsync();
        await PersonalDetailsPage.SavePersonalDetailsAsync();
        await PersonalDetailsPage.ExpectPersonalDetailsUpdatedAsync();
        await PersonalDetailsPage.OpenAttachmentFormAsync();
        await PersonalDetailsPage.UploadFileAndMakeCommentAsync(filePath, attachmentComment);
        await PersonalDetailsPage.ExpectAttachmentSavedAsync();
    }

    private async Task CreateAdminUserAndSearchAsync(string username)
    {
        await AddUserPage.OpenAddUserFormAsync();
        await AddUserPage.CreateAdminUserAsync(username, EmployeeName, UserPassword);
        TrackUserForCleanup(username);
        await AddUserPage.SearchUserAsync(username);
    }
}
