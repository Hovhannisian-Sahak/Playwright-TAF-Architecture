using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Core.Authentication;
using PlaywrightTAF.Core.Configuration;

namespace PlaywrightTAF.Tests.Authentication;

[TestFixture]
public class AuthStateGeneratorTests
{
    [Test]
    public async Task GenerateAuthStates()
    {
        await AuthSetup.CreateAuthStateAsync(ConfigurationReader.Current.Admin, AuthStatePaths.Admin);
        await AuthSetup.CreateAuthStateAsync(ConfigurationReader.Current.User, AuthStatePaths.User);
    }
}
