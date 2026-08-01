using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

[SetUpFixture]
public sealed class TestRunCleanup
{
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        AuthenticatedUiBaseTest.DeleteCreatedAuthStates();
    }
}
