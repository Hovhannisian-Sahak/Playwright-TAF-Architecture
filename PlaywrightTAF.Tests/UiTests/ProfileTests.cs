using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;
using PlaywrightTAF.UI.Pages;

namespace PlaywrightTAF.Tests.UiTests;

public class ProfileTests : UserTest
{
    [Test]
    [Category("UI")]
    public async Task UserCanOpenProfile()
    {
        var profilePage = new ProfilePage(Page);

        await profilePage.OpenAsync();

        Assert.That(await profilePage.IsLoadedAsync(), Is.True);
    }
}
