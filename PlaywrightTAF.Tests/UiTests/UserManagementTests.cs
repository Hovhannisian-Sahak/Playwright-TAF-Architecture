using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class UserManagementTests : AdminTest
{
    private const string EmployeeName = "Ranga  Akunuri";
    private const string UserPassword = "TestUser123!@#Aa";
    private const string ChangePassword = "TestUser123!@#Aab";

    private AddUserPage addUserPage = null!;
    private DeleteUserPage deleteUserPage = null!;
    private EditUserPage editUserPage = null!;
    private PersonalDetailsPage personalDetailsPage = null!;

    [SetUp]
    public void SetUpPages()
    {
        addUserPage = new AddUserPage(Page);
        deleteUserPage = new DeleteUserPage(Page);
        editUserPage = new EditUserPage(Page);
        personalDetailsPage = new PersonalDetailsPage(Page);
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanAddUser()
    {
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await addUserPage.OpenAddUserFormAsync();
        await addUserPage.CreateAdminUserAsync(newUsername, EmployeeName, UserPassword);
        await addUserPage.SearchUserAsync(newUsername);

        await addUserPage.ExpectUserExistsAsync(newUsername);

        await deleteUserPage.DeleteFirstSearchResultAsync();
    }

    [Test]
    [Category("UI")]
    public async Task AdminCanDeleteUser()
    {
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

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
        var newUsername = $"Adminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var changedUsername = $"ChangedAdminn{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

        await addUserPage.OpenAddUserFormAsync();
        await addUserPage.CreateAdminUserAsync(newUsername, EmployeeName, UserPassword);
        await editUserPage.SearchUserAsync(newUsername);

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
        var lastName = $"Admin{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        string filePath = Path.Combine(AppContext.BaseDirectory, "test.png");

        await personalDetailsPage.OpenPersonalDetailsAsync();
        await personalDetailsPage.FillLastNameAsync(lastName);
        await personalDetailsPage.SelectNationalityAsync("Armenian");
        await personalDetailsPage.SetBirthDateAsync();
        await personalDetailsPage.SavePersonalDetailsAsync();
        await personalDetailsPage.OpenAttachmentFormAsync();
        await personalDetailsPage.UploadFileAndMakeCommentAsync(filePath, "Test");
    }
}
