using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Security.Claims;
using ClaimTypes = ISynergy.Framework.Core.Constants.ClaimTypes;

namespace ISynergy.Framework.Core.Extensions.Tests
{
    [TestClass()]
    public class ClaimsPrincipalExtensionsTests
    {
        private Guid userId = Guid.NewGuid();
        private string userName = "userName";
        private Guid groupName = Guid.NewGuid();

        private readonly ClaimsPrincipal _principal;

        public ClaimsPrincipalExtensionsTests()
        {
            var identity = new ClaimsIdentity(
                        "OAuth",
                        ClaimTypes.UserNameType,
                        ClaimTypes.RoleType);

            identity.AddClaim(new Claim(ClaimTypes.AccountIdType, groupName.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.AccountDescriptionType, "Test"));
            identity.AddClaim(new Claim(ClaimTypes.UserNameType, userName));
            identity.AddClaim(new Claim(ClaimTypes.UserIdType, userId.ToString()));

            _principal = new ClaimsPrincipal(identity);
        }

        [TestMethod()]
        public void GetClaimsTest()
        {
            Assert.IsTrue(_principal.GetClaims(ClaimTypes.UserNameType).Any());
        }

        [TestMethod()]
        public void HasClaimTest()
        {
            Assert.IsTrue(_principal.HasClaim(ClaimTypes.AccountDescriptionType));
            Assert.IsFalse(_principal.HasClaim(ClaimTypes.TimeZoneType));
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
}