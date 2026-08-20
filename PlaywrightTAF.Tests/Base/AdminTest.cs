using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;
using PlaywrightTAF.Core.Logging;
using PlaywrightTAF.UI.Pages.UserManagementPages;
using Serilog;

namespace PlaywrightTAF.Tests.Base;

public abstract class AdminTest : AuthenticatedUiBaseTest
{
    private static readonly ILogger Logger = LogProvider.ForContext<AdminTest>();
    private readonly List<string> _personalAttachmentCommentsToDelete = [];
    private readonly List<string> _usernamesToDelete = [];
    private PersonalDetailsSnapshot? _personalDetailsSnapshot;

    protected override Credentials Credentials => ConfigurationReader.Current.Admin;

    protected override string StorageStatePath => AuthStatePaths.CurrentRunAdmin;

    protected void TrackUserForCleanup(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || _usernamesToDelete.Contains(username))
        {
            return;
        }

        _usernamesToDelete.Add(username);
    }

    protected void UntrackUserFromCleanup(string username)
    {
        _usernamesToDelete.Remove(username);
    }

    protected async Task TrackPersonalDetailsForCleanupAsync()
    {
        var personalDetailsPage = PageObject<PersonalDetailsPage>();
        await personalDetailsPage.OpenPersonalDetailsAsync();

        _personalDetailsSnapshot = new PersonalDetailsSnapshot(
            await personalDetailsPage.GetLastNameAsync(),
            await personalDetailsPage.GetNationalityAsync(),
            await personalDetailsPage.GetBirthDateAsync());
    }

    protected void TrackPersonalAttachmentForCleanup(string comment)
    {
        if (string.IsNullOrWhiteSpace(comment) || _personalAttachmentCommentsToDelete.Contains(comment))
        {
            return;
        }

        _personalAttachmentCommentsToDelete.Add(comment);
    }

    protected override async Task CleanupTestDataAsync()
    {
        await DeleteTrackedPersonalAttachmentsAsync();
        await RestorePersonalDetailsAsync();
        await DeleteTrackedUsersAsync();
    }

    private async Task DeleteTrackedPersonalAttachmentsAsync()
    {
        for (int index = _personalAttachmentCommentsToDelete.Count - 1; index >= 0; index--)
        {
            string comment = _personalAttachmentCommentsToDelete[index];

            try
            {
                var personalDetailsPage = PageObject<PersonalDetailsPage>();
                await personalDetailsPage.OpenPersonalDetailsAsync();
                await personalDetailsPage.DeleteAttachmentByCommentAsync(comment);
                Logger.Information("Deleted personal attachment with comment {Comment}", comment);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Could not delete personal attachment with comment {Comment}", comment);
            }
        }

        _personalAttachmentCommentsToDelete.Clear();
    }

    private async Task RestorePersonalDetailsAsync()
    {
        if (_personalDetailsSnapshot is null)
        {
            return;
        }

        try
        {
            var personalDetailsPage = PageObject<PersonalDetailsPage>();
            await personalDetailsPage.OpenPersonalDetailsAsync();
            await personalDetailsPage.FillLastNameAsync(_personalDetailsSnapshot.LastName);

            if (!string.IsNullOrWhiteSpace(_personalDetailsSnapshot.Nationality))
            {
                await personalDetailsPage.SelectNationalityAsync(_personalDetailsSnapshot.Nationality);
            }

            if (!string.IsNullOrWhiteSpace(_personalDetailsSnapshot.BirthDate))
            {
                await personalDetailsPage.SetBirthDateValueAsync(_personalDetailsSnapshot.BirthDate);
            }

            await personalDetailsPage.SavePersonalDetailsAsync();
            await personalDetailsPage.ExpectPersonalDetailsUpdatedAsync();

            Logger.Information("Restored personal details after UI test.");
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Could not restore personal details after UI test.");
        }
        finally
        {
            _personalDetailsSnapshot = null;
        }
    }

    private async Task DeleteTrackedUsersAsync()
    {
        for (int index = _usernamesToDelete.Count - 1; index >= 0; index--)
        {
            string username = _usernamesToDelete[index];

            try
            {
                var deleteUserPage = PageObject<DeleteUserPage>();
                await deleteUserPage.SearchUserAsync(username);
                await deleteUserPage.DeleteFirstSearchResultAsync();
                Logger.Information("Deleted UI-created user {Username}", username);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Could not delete UI-created user {Username}", username);
            }
        }

        _usernamesToDelete.Clear();
    }

    private sealed record PersonalDetailsSnapshot(
        string LastName,
        string Nationality,
        string BirthDate);
}
