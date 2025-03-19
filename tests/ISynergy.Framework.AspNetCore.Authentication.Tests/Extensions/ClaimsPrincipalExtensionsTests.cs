using ISynergy.Framework.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ISynergy.Framework.AspNetCore.Authentication.Tests.Extensions;

[TestClass()]
public class ClaimsPrincipalExtensionsTests
{
    private string userId = Guid.NewGuid().ToString();
    private string userName = "userName";
    private Guid groupName = Guid.NewGuid();

    private readonly ClaimsPrincipal _principal;

    public ClaimsPrincipalExtensionsTests()
    {
        ClaimsIdentity identity = new(
                    "OAuth",
                    Claims.Username,
                    Claims.Role);

        identity.AddClaim(new Claim(Claims.KeyId, groupName.ToString()));
        identity.AddClaim(new Claim(Claims.Name, "Test"));
        identity.AddClaim(new Claim(Claims.Username, userName));
        identity.AddClaim(new Claim(Claims.Subject, userId.ToString()));

        _principal = new ClaimsPrincipal(identity);
    }

    [TestMethod()]
    public void GetClaimsTest()
    {
        Assert.IsTrue(_principal.GetClaims(Claims.Username).Any());
    }

    [TestMethod()]
    public void HasClaimTest()
    {
        Assert.IsTrue(_principal.HasClaim(Claims.Name));
        Assert.IsFalse(_principal.HasClaim(Claims.Zoneinfo));
    }

    [TestMethod()]
    public void GetUserIdTest()
    {
        Assert.AreEqual(userId, _principal.GetUserId());
    }

    [TestMethod()]
    public void GetUserNameTest()
    {
        Assert.AreEqual(userName, _principal.GetUserName());
    }

    [TestMethod()]
    public void GetAccountIdTest()
    {
        Assert.AreEqual(groupName, _principal.GetAccountId());
    }
}