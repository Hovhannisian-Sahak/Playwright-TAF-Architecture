using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.Base;

public abstract class AdminTest : AuthenticatedUiBaseTest
{
    protected override Credentials Credentials => ConfigurationReader.Current.Admin;

    protected override string StorageStatePath => AuthStatePaths.Admin;
}
