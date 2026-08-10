using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.Tests.TestData;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserManagementTests : AdminTest
{
    private const string EmployeeName = "Ranga  Akunuri";
    private const string UserPassword = "TestUser123!@#Aa";
    private const string ChangePassword = "TestUser123!@#Aab";

    private readonly AddUserPage addUserPage;
    private readonly DeleteUserPage deleteUserPage;
    private readonly EditUserPage editUserPage;
    private readonly PersonalDetailsPage personalDetailsPage;

    public UserManagementTests()
    {
        addUserPage = PageObject<AddUserPage>();
        deleteUserPage = PageObject<DeleteUserPage>();
        editUserPage = PageObject<EditUserPage>();
        personalDetailsPage = PageObject<PersonalDetailsPage>();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanAddUser()
    {
        var newUsername = TestDataFactory.UniqueUsername("Adminn");

        await CreateAdminUserAndSearchAsync(newUsername);

        await addUserPage.ExpectUserExistsAsync(newUsername);

        await deleteUserPage.DeleteFirstSearchResultAsync();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanDeleteUser()
    {
        var newUsername = TestDataFactory.UniqueUsername("Adminn");

        await addUserPage.OpenAddUserFormAsync();
        await addUserPage.CreateAdminUserAsync(newUsername, EmployeeName, UserPassword);
        await deleteUserPage.SearchUserAsync(newUsername);

        await deleteUserPage.ExpectUserExistsAsync(newUsername);

        await deleteUserPage.DeleteFirstSearchResultAsync();
        await deleteUserPage.SearchUserAsync(newUsername);

        await deleteUserPage.ExpectUserDoesNotExistAsync(newUsername);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanChangeUserNameAndPassword()
    {
        var newUsername = TestDataFactory.UniqueUsername("Adminn");
        var changedUsername = TestDataFactory.UniqueUsername("ChangedAdminn");

        await CreateAdminUserAndSearchAsync(newUsername);

        await editUserPage.ExpectUserExistsAsync(newUsername);

        await editUserPage.EditFirstSearchResultAsync(changedUsername, ChangePassword);
        await editUserPage.SearchUserAsync(changedUsername);

        await editUserPage.ExpectUserExistsAsync(changedUsername);
        await deleteUserPage.DeleteFirstSearchResultAsync();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanEditInfo()
    {
        var lastName = TestDataFactory.UniqueUsername("Admin");
        string filePath = TestDataFactory.UploadFilePath();

        await personalDetailsPage.OpenPersonalDetailsAsync();
        await personalDetailsPage.FillLastNameAsync(lastName);
        await personalDetailsPage.SelectNationalityAsync("Armenian");
        await personalDetailsPage.ExpectNationalityAsync("Armenian");
        await personalDetailsPage.SetBirthDateAsync();
        await personalDetailsPage.ExpectBirthDateAsync();
        await personalDetailsPage.SavePersonalDetailsAsync();
        await personalDetailsPage.ExpectPersonalDetailsUpdatedAsync();
        await personalDetailsPage.OpenAttachmentFormAsync();
        await personalDetailsPage.UploadFileAndMakeCommentAsync(filePath, "Test");
        await personalDetailsPage.ExpectAttachmentSavedAsync();
    }

    private async Task CreateAdminUserAndSearchAsync(string username)
    {
        await addUserPage.OpenAddUserFormAsync();
        await addUserPage.CreateAdminUserAsync(username, EmployeeName, UserPassword);
        await addUserPage.SearchUserAsync(username);
    }
}
