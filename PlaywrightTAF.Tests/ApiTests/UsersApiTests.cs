using System.Threading.Tasks;
using NUnit.Framework;
using PlaywrightTAF.Tests.Base;

namespace PlaywrightTAF.Tests.ApiTests;

public class UserTests : BaseApiTest
{
    [Test]
    public async Task GetCurrentUser_ShouldReturnUser()
    {
        var user = await UserService.GetCurrentUser();

        Assert.Multiple(() =>
        {
            Assert.That(user.email, Is.EqualTo(TestEmail));
            Assert.That(user.username, Is.EqualTo(TestUsername));
        });
    }
}
