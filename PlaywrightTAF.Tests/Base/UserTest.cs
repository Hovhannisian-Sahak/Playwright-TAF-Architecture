using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.Base;

public abstract class UserTest : AuthenticatedUiBaseTest
{
    protected override Credentials Credentials => ConfigurationReader.Current.User;

    protected override string StorageStatePath => AuthStatePaths.User;
}
