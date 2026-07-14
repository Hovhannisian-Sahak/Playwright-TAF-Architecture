using Microsoft.Playwright;
using PlaywrightTAF.Core.Authentication;

namespace PlaywrightTAF.Tests.Base;

public abstract class UserTest : UiBaseTest
{
    protected override BrowserNewContextOptions CreateContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = Configuration.BaseUrl,
            StorageStatePath = AuthStatePaths.User
        };
    }
}
