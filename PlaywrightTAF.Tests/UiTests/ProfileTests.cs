using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

namespace PlaywrightTAF.Tests.UiTests;

public class ProfileTests : UserTest
{
    [Test]
    public async Task UserCanOpenProfile()
    {
        await Page.GotoAsync("/profile");

        Assert.That(Page.Url,
            Does.Contain("profile"));
    }
}
